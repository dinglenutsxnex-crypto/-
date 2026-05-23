import { AbstractMesh, Skeleton, Scene, TransformNode } from "@babylonjs/core";
import { Gender } from "../../sf3DTO/Gender";
import { EquipmentType } from "../Items/EquipmentType";
import { resolveModelConfig, loadSkinGlb } from "./ModelObject";
import { bindToMaster } from "./ModelSkin";

export interface ModelAssemblyResult {
  rootNodes: TransformNode[];
  meshes: AbstractMesh[];
  skeleton: Skeleton | null;
}

function buildNodeMap(root: TransformNode): Map<string, TransformNode> {
  const map = new Map<string, TransformNode>();
  function walk(n: TransformNode) {
    map.set(n.name, n);
    for (const child of n.getChildren()) {
      if (child instanceof TransformNode) walk(child);
    }
  }
  walk(root);
  return map;
}

async function loadModelParts(
  modelName: string,
  gender: Gender,
  scene: Scene,
) {
  const parts = resolveModelConfig(modelName, gender);
  return Promise.all(parts.map(p => loadSkinGlb(p, scene)));
}

export async function assembleCharacter(
  scene: Scene,
  gender: Gender,
  headModel: string,
  equipped: { type: EquipmentType; model: string }[],
): Promise<ModelAssemblyResult> {
  const armor = equipped.find(e => e.type === EquipmentType.Armor);
  const bodyModel = armor?.model ?? "arm__base";

  let masterSkeleton: Skeleton | null = null;
  let masterArmature: TransformNode | null = null;
  let masterNodeMap: Map<string, TransformNode> = new Map();
  const allMeshes: AbstractMesh[] = [];

  // Load body parts — first glb establishes master
  const bodyParts = await loadModelParts(bodyModel, gender, scene);
  for (const bp of bodyParts) {
    if (!masterSkeleton && bp.skeletons.length > 0) {
      masterSkeleton = bp.skeletons[0];
      masterArmature = bp.armature;
      if (masterArmature) masterNodeMap = buildNodeMap(masterArmature);
      allMeshes.push(...bp.meshes);
    } else if (masterSkeleton && masterArmature) {
      for (let i = 0; i < bp.meshes.length; i++) {
        masterSkeleton = bp.skeletons[i] ?? masterSkeleton;
        bindToMaster(bp.meshes[i], bp.skeletons[i] ?? masterSkeleton, masterNodeMap, masterArmature);
      }
      allMeshes.push(...bp.meshes);
    }
  }

  // Load head — bind to master
  if (headModel && masterSkeleton && masterArmature) {
    const headParts = await loadModelParts(headModel, gender, scene);
    for (const hp of headParts) {
      for (let i = 0; i < hp.meshes.length; i++) {
        bindToMaster(hp.meshes[i], hp.skeletons[i] ?? masterSkeleton, masterNodeMap, masterArmature);
      }
      allMeshes.push(...hp.meshes);
    }
  }

  // Load remaining equipment — bind to master
  for (const eq of equipped) {
    if (eq.type === EquipmentType.Armor || !eq.model || !masterSkeleton || !masterArmature) continue;
    const eqParts = await loadModelParts(eq.model, gender, scene);
    for (const ep of eqParts) {
      for (let i = 0; i < ep.meshes.length; i++) {
        bindToMaster(ep.meshes[i], ep.skeletons[i] ?? masterSkeleton, masterNodeMap, masterArmature);
      }
      allMeshes.push(...ep.meshes);
    }
  }

  // Rotate master armature so character faces -Z
  const rootNodes: TransformNode[] = [];
  if (masterArmature) {
    masterArmature.rotationQuaternion = null;
    masterArmature.rotation.y = Math.PI / 2;
    rootNodes.push(masterArmature);
  }

  return { rootNodes, meshes: allMeshes, skeleton: masterSkeleton };
}
