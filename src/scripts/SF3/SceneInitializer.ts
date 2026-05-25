import { Vector3, FreeCamera, HemisphericLight, DirectionalLight, Scene, Bone, TransformNode, Quaternion } from "@babylonjs/core";
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
      // Unity is left-handed (Z-forward); glTF / BabylonJS bone TN local space is
      // right-handed (Z-backward).  The glTF loader applies the handedness fix only
      // at the __root__ level, so individual TN local transforms still live in
      // right-handed glTF space.  We must convert Unity → glTF before writing:
      //   position : negate Z
      //   quaternion: negate X and Y  (reflects the Z-axis flip onto the rotation)
      tn.position = new Vector3(t.position.x, t.position.y, -t.position.z);
      tn.rotationQuaternion = new Quaternion(-t.rotation.x, -t.rotation.y, t.rotation.z, t.rotation.w);
    }

    for (const sk of scene.skeletons) {
      (sk as any)._isDirty = true;
      (sk as any).prepare(true);
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
