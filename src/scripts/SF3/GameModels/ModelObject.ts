import {
  SceneLoader,
  AbstractMesh,
  Skeleton,
  TransformNode,
  PBRMaterial,
  Texture,
  Scene,
} from "@babylonjs/core";
import "@babylonjs/loaders";
import { Gender } from "../../sf3DTO/Gender";

export interface SkinPart {
  skinType: string;
  glbDir: string;
  textureBase: string;
}

const SKIN_CONFIG: Record<string, { skinType: string; parts: string[]; textureBase: string }> = {
  "arm__base_m": { skinType: "arm", parts: ["body", "pants_l", "pants_r"], textureBase: "arm__base_m" },
  "arm__base_f": { skinType: "arm", parts: ["body", "pants_l", "pants_r"], textureBase: "arm__base_f" },
  "arm__str_15_m": { skinType: "arm", parts: ["body", "hand_l", "hand_r", "shoulders", "tie"], textureBase: "arm__str_15_m" },
  "head__01a_m": { skinType: "head", parts: ["head__01a_m"], textureBase: "head__01a_m" },
  "head__01a_f": { skinType: "head", parts: ["head__01a_f"], textureBase: "head__01a_f" },
  "hair__01_m":  { skinType: "hair", parts: ["hair__01_m"], textureBase: "hair__m_01" },
  "hair__01_f":  { skinType: "hair", parts: ["hair__01_f"], textureBase: "hair__f_01" },
  "skeleton":    { skinType: "", parts: [], textureBase: "" },
};

const SKIN_TYPE_PREFIX: Record<string, string> = {
  arm: "arm", head: "head", hair: "hair", helm: "helm",
  wpn: "wpn", rng: "rng", mgc: "mgc",
};

export function resolveModelConfig(modelName: string, gender: Gender): SkinPart[] {
  const suffix = gender === Gender.Female ? "_f" : "_m";
  const key = `${modelName.replace(/-/g, "__")}${suffix}`;
  const cfg = SKIN_CONFIG[key];
  if (!cfg) return [];
  const prefix = SKIN_TYPE_PREFIX[cfg.skinType] ?? cfg.skinType;
  return cfg.parts.map(part => {
    const isMulti = part !== key;
    const glbDir = isMulti
      ? `assets/skins/${prefix}/${key}__${part}/`
      : `assets/skins/${prefix}/${part}/`;
    return { skinType: cfg.skinType, glbDir, textureBase: cfg.textureBase };
  });
}

function applySkinTexture(mesh: AbstractMesh, scene: Scene, textureBase: string, glbDir: string): void {
  const pbr = mesh.material as PBRMaterial;
  if (!pbr) return;
  const cmPath = `${glbDir}textures/${textureBase}_CM.png`;
  const cmTex = new Texture(cmPath, scene, false, false);
  pbr.albedoTexture = cmTex;
  pbr.useAlphaFromAlbedoTexture = false;
  pbr.metallic = 0;
  pbr.roughness = 0.8;
}

export async function loadSkinGlb(
  part: SkinPart,
  scene: Scene,
): Promise<{
  meshes: AbstractMesh[];
  skeletons: Skeleton[];
  armature: TransformNode | null;
}> {
  const result = await SceneLoader.ImportMeshAsync("", part.glbDir, "mesh.glb", scene);
  for (const m of result.meshes) {
    applySkinTexture(m, scene, part.textureBase, part.glbDir);
  }
  const armature = result.transformNodes.find((n: TransformNode) => n.name === "Armature") ?? null;
  return { meshes: result.meshes, skeletons: result.skeletons, armature };
}
