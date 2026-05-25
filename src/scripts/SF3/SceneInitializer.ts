import { Vector3, FreeCamera, HemisphericLight, DirectionalLight, Scene, Bone, TransformNode, Quaternion, Matrix } from "@babylonjs/core";
import "@babylonjs/loaders";
import { SceneManager } from "../core/SceneManager";
import { Gender } from "../sf3DTO/Gender";
import { EquipmentType } from "./Items/EquipmentType";
import { assembleCharacter } from "./GameModels/ModelComponents";
import { AnimationBinaries } from "./Moves/AnimationBinaries";

async function loadBytes(path: string): Promise<Uint8Array> {
  const res = await fetch(path);
  if (!res.ok) throw new Error(`Failed to load ${path}`);
  return new Uint8Array(await res.arrayBuffer());
}

function parseSkeletonIds(xmlText: string): Map<number, string> {
  const map = new Map<number, string>();
  const regex = /<Bone\s+Name="([^"]+)"\s+ID="(\d+)"/g;
  let match: RegExpExecArray | null;
  while ((match = regex.exec(xmlText)) !== null) {
    map.set(parseInt(match[2], 10), match[1]);
  }
  return map;
}

export async function initializeScene(mgr: SceneManager): Promise<void> {
  const scene = mgr.scene;
  setupCamera(scene);
  setupLights(scene);

  const result = await assembleCharacter(scene, Gender.Male, "head__01a", [
    { type: EquipmentType.Armor, model: "arm__base" },
    { type: EquipmentType.Helmet, model: "hair-01" },
  ]);

  console.log("Character assembled:", {
    meshes: result.meshes.map(m => m.name),
    skeleton: result.skeleton?.name,
    roots: result.rootNodes.map(n => n.name),
  });

  // Load animation data
  const [skeletonXml, animBytes] = await Promise.all([
    loadBytes("assets/configs/content/bones/configs/skeleton.txt").then(b => new TextDecoder().decode(b)),
    loadBytes("assets/animations/m_idle.bytes"),
  ]);

  const idToName = parseSkeletonIds(skeletonXml);
  const anim = AnimationBinaries.LoadFromBytes(animBytes, "agl_super_1_kick_1.bytes");
  if (!anim) {
    console.error("Failed to parse animation");
    return;
  }

  console.log("Animation loaded:", {
    frames: anim.frames.length,
    bones: anim.bonesIDs.length,
    boneIds: anim.bonesIDs,
    hasTangents: !!anim.animationTangents,
  });

  // Build boneName → BabylonJS bone from ALL skeletons (covers body + head + equipment)
  const boneByName = new Map<string, Bone>();
  for (const sk of scene.skeletons) {
    for (const b of sk.bones) {
      if (!boneByName.has(b.name)) boneByName.set(b.name, b);
    }
  }

  // Debug match rate
  let matched = 0;
  const unmatched: number[] = [];
  for (const id of anim.bonesIDs) {
    const bname = idToName.get(id);
    if (bname && boneByName.has(bname)) matched++;
    else unmatched.push(id);
  }
  console.log(`Animation bone match: ${matched}/${anim.bonesIDs.length} matched, ${unmatched.length} unmatched:`, unmatched);

  const skel = result.skeleton;
  if (!skel) {
    console.error("No skeleton");
    return;
  }

  let debugged = false;

  // Track frame timing
  let currentFrame = 0;
  const frameCount = anim.frames.length;
  let lastTime = performance.now();

  scene.onBeforeRenderObservable.add(() => {
    const now = performance.now();
    const dt = (now - lastTime) / 1000;
    lastTime = now;

    currentFrame = (currentFrame + dt * 30) % frameCount;
    const frameIdx = Math.floor(currentFrame);

    const frame = anim.frames[frameIdx];
    for (let i = 0; i < anim.bonesIDs.length; i++) {
      const boneID = anim.bonesIDs[i];
      const boneName = idToName.get(boneID);
      if (!boneName) continue;

      const bone = boneByName.get(boneName);
      if (!bone) continue;

      const tn = bone.getTransformNode();
      if (!tn) continue;

      const t = frame.bonesAnimation[i];
      tn.position = t.position;
      tn.rotationQuaternion = t.rotation;
    }

    // Force skeleton matrix recompute — TN changes don't always dirty the skeleton
    for (const sk of scene.skeletons) {
      (sk as any)._isDirty = true;
      (sk as any).prepare(true);
    }

    if (!debugged) {
      debugged = true;
      const chain = ["pelvis","stomach","chest","neck","head"];
      for (const name of chain) {
        const bone = boneByName.get(name);
        if (!bone) { console.log(name + ': NO BONE'); continue; }
        const tn = bone.getTransformNode();
        if (!tn) { console.log(name + ': NO TN'); continue; }
        // Read ALL data
        const b = bone as any;
        const ibq = new Quaternion(); b.getAbsoluteInverseBindMatrix().decompose(undefined, ibq, undefined);
        const tnq = tn.rotationQuaternion!;
        tn.computeWorldMatrix(true);
        const wq = new Quaternion(); tn.getWorldMatrix().decompose(undefined, wq, undefined);
        const fq = new Quaternion(); b.getFinalMatrix().decompose(undefined, fq, undefined);
        // Skinning matrix from _transformMatrices
        let sq = "N/A"; let sv = "N/A";
        const sk = result.skeleton as any;
        if (sk._transformMatrices) {
          const idx = b._index ?? sk.bones.indexOf(bone);
          if (idx >= 0 && idx * 16 + 16 <= sk._transformMatrices.length) {
            const sm = Matrix.FromArray(sk._transformMatrices, idx * 16);
            const sqq = new Quaternion(); const svv = new Vector3();
            sm.decompose(svv, sqq, undefined);
            sq = `(${sqq.x.toFixed(4)},${sqq.y.toFixed(4)},${sqq.z.toFixed(4)},${sqq.w.toFixed(4)})`;
            sv = `(${svv.x.toFixed(2)},${svv.y.toFixed(2)},${svv.z.toFixed(2)})`;
          }
        }
        // Parent chain for bone
        let par = ""; let p = bone.getParent(); while (p) { par = p.name + "→" + par; p = p.getParent(); }
        console.log(`${name} invBind=(${ibq.x.toFixed(4)},${ibq.y.toFixed(4)},${ibq.z.toFixed(4)},${ibq.w.toFixed(4)}) tnLocal=(${tnq.x.toFixed(4)},${tnq.y.toFixed(4)},${tnq.z.toFixed(4)},${tnq.w.toFixed(4)}) tnWorld=(${wq.x.toFixed(4)},${wq.y.toFixed(4)},${wq.z.toFixed(4)},${wq.w.toFixed(4)}) boneFinal=(${fq.x.toFixed(4)},${fq.y.toFixed(4)},${fq.z.toFixed(4)},${fq.w.toFixed(4)}) skinMat=${sq} pos=${sv} parent=[${par}]`);
      }
    }
  });
}

function setupCamera(scene: Scene): void {
  const cam = new FreeCamera("cam", new Vector3(0, 155, -950), scene);
  cam.setTarget(Vector3.Zero());
  cam.minZ = 10;
  cam.maxZ = 5000;
  cam.fov = 30 * Math.PI / 180;
  scene.activeCamera = cam;
}

function setupLights(scene: Scene): void {
  new HemisphericLight("hemi", new Vector3(0, 1, 0), scene);
  const dir = new DirectionalLight("dir", new Vector3(-0.5, -1, -0.3), scene);
  dir.position.set(300, 500, 300);
}
