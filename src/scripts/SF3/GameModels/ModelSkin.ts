import { AbstractMesh, Skeleton, TransformNode, Bone } from "@babylonjs/core";

/**
 * Relink a skin part's mesh and skeleton to use the master hierarchy's bone Transforms.
 * Mirrors Unity's ModelSkin.BoundToBones() — keeps the mesh's own bind matrices
 * but re-points each bone's TransformNode to the equivalent-named bone in the master.
 */
export function bindToMaster(
  mesh: AbstractMesh,
  skeleton: Skeleton,
  masterNodeMap: Map<string, TransformNode>,
  masterArmature: TransformNode,
): void {
  // Reparent mesh to equivalent bone in master hierarchy
  const origParent = mesh.parent;
  if (origParent && origParent instanceof TransformNode) {
    const equiv = masterNodeMap.get(origParent.name);
    mesh.parent = equiv ?? masterArmature;
  } else {
    mesh.parent = masterArmature;
  }

  // Relink each bone's TransformNode to the master hierarchy
  for (const bone of skeleton.bones) {
    const tn = bone.getTransformNode();
    if (tn) {
      const masterTn = masterNodeMap.get(tn.name);
      if (masterTn) bone.linkTransformNode(masterTn);
    }
  }
}
