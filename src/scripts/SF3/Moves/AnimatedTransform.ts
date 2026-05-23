import { Vector3, Quaternion } from "@babylonjs/core";

export class AnimatedTransform {
  static readonly IDENTITY = new AnimatedTransform(Vector3.Zero(), Quaternion.Identity());

  animateThisFrame: boolean = false;

  private _position: Vector3;
  private _rotation: Quaternion;

  get position(): Vector3 { return this._position; }
  get rotation(): Quaternion { return this._rotation; }

  constructor(newPosition?: Vector3, newRotation?: Quaternion) {
    this._position = newPosition ?? Vector3.Zero();
    this._rotation = newRotation ?? Quaternion.Identity();
  }

  SetRotation(newRotation: Quaternion): void {
    this._rotation = newRotation;
  }

  SetPosition(newPosition: Vector3): void {
    this._position = newPosition;
  }

  AddPosition(addToPos: Vector3): void {
    this._position = this._position.add(addToPos);
  }

  AddRotation(addRotation: Quaternion): void {
    this._rotation = this._rotation.multiply(addRotation);
  }

  static CopyBoneTransform(from: AnimatedTransform, to: AnimatedTransform): void {
    to.SetPosition(from.position);
    to.SetRotation(from.rotation);
  }
}
