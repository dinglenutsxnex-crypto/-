import { TransformNode, Vector3, Quaternion } from "@babylonjs/core";

export class Bone {
  private _transform: TransformNode;
  private _lastPosition: Vector3 = Vector3.Zero();

  animatedThisFrame: boolean = false;
  mirrorBone: Bone | null = null;
  parentBone: Bone | null = null;
  childBones: Bone[] = [];
  pseudoPhysics: boolean = false;
  boneID: number;
  weight: number = 0;
  startPosition: Vector3;
  startRotation: Vector3;
  previousPosition: Vector3 = Vector3.Zero();
  previousLocalPosition: Vector3 = Vector3.Zero();

  get transform(): TransformNode { return this._transform; }
  get boneName(): string { return this._transform.name; }
  get localPosition(): Vector3 { return this._transform.position.clone(); }
  get position(): Vector3 { return this._transform.absolutePosition.clone(); }
  get localRotation(): Quaternion { return this._transform.rotationQuaternion ?? Quaternion.Identity(); }
  get lossyScale(): Vector3 { return this._transform.scaling.clone(); }
  get localScale(): Vector3 { return this._transform.scaling.clone(); }

  onPreviousPositionUpdate: (() => void) | null = null;

  constructor(transform: TransformNode, newBoneID: number = -1, newParentBone: Bone | null = null) {
    this._transform = transform;
    this.childBones = [];
    this.startPosition = transform.position.clone();
    this.startRotation = new Vector3(
      transform.rotation.x, transform.rotation.y, transform.rotation.z
    );
    this.boneID = newBoneID;
    this.parentBone = newParentBone;
    this.animatedThisFrame = false;
    this.weight = 0;
    this.pseudoPhysics = false;
    this.UpdatePreviousPosition();
  }

  SetPseudoPhysics(value: boolean): void {
    this.pseudoPhysics = value;
  }

  SetWeight(newWeight: number): void {
    this.weight = newWeight;
  }

  SetBoneID(newID: number): void {
    this.boneID = newID;
  }

  SetPosition(position: Vector3, isPrevious: boolean = true): void {
    if (isPrevious) this.UpdatePreviousPosition();
    this._transform.setAbsolutePosition(position);
  }

  SetRotation(value: Quaternion): void {
    this._transform.rotationQuaternion = value;
  }

  SetLocalPosition(position: Vector3): void {
    this._transform.position = position;
    this.animatedThisFrame = true;
  }

  SetLocalRotation(vector: Vector3): void {
    this._transform.rotation = vector;
  }

  SetLocalRotationQuat(rotation: Quaternion): void {
    this._transform.rotationQuaternion = rotation;
  }

  UpdatePreviousPosition(): void {
    if (this.onPreviousPositionUpdate) this.onPreviousPositionUpdate();
    this.previousPosition = this._transform.absolutePosition.clone();
    this.previousLocalPosition = this._transform.position.clone();
  }

  SetPreviousPosition(position: Vector3): void {
    this.previousPosition = position;
  }

  ShiftPosition(vector: Vector3): void {
    const pos = this._transform.absolutePosition.add(vector);
    this._transform.setAbsolutePosition(pos);
  }

  ResetAnimated(): void {
    this.animatedThisFrame = false;
  }

  ResetPosition(): void {
    this.SetLocalPosition(this.startPosition);
    this.SetLocalRotation(this.startRotation);
  }
}
