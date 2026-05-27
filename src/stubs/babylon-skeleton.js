export class Skeleton {
  constructor(name, id, scene) {
    this.name = name;
    this.id = id;
    this.bones = [];
    this._scene = scene;
    this._isEnabled = true;
    this._ranges = {};
    this._absoluteTransformIsDirty = false;
    this._canUseTextureForBones = false;
    this._uniqueId = 0;
    this._numBonesWithLinkedTransformNodes = 0;
    this._meshesWithPoseMatrix = new Map();
  }
  get isEnabled() { return this._isEnabled; }
  set isEnabled(v) { this._isEnabled = v; }
  getTransformNode() { return null; }
  prepare() {}
  getAnimatables() { return []; }
  getAnimationPropertiesOverride() { return null; }
  setAnimationPropertiesOverride(o) {}
  serialize() { return null; }
  getHierarchy() { return this.bones; }
  getChildren() { return this.bones; }
  getBoneIndex(bone) { return this.bones.indexOf(bone); }
  getBoneByName(name) { return this.bones.find(b => b.name === name) || null; }
  getBone(boneIdx) { return this.bones[boneIdx] || null; }
  getBoneByUsedNodeId(nodeId) { return null; }
  beginAnimation(name, loop, speedRatio, onEnd) {}
  stopAnimation() {}
  _markAsDirty() {}
  _numBonesWithLinkedTransformNodes = 0;
}
