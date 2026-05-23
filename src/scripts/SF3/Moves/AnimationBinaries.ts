import { Vector3, Quaternion } from "@babylonjs/core";
import { BinaryReaderNekki } from "../../utils/BinaryReaderNekki";
import { AnimationFrame } from "./AnimationFrame";
import { AnimatedTransform } from "./AnimatedTransform";

export interface IHasFileName {
  FileName: string;
}

export class AnimationBinaries implements IHasFileName {
  private _fileName: string = "";

  get name(): string {
    return this._fileName.substring(0, this._fileName.lastIndexOf("."));
  }

  frames: AnimationFrame[] = [];
  bonesIDs: number[] = [];
  animationTangents: AnimationBinaries | null = null;
  FileName: string = "";

  get bonesCount(): number {
    return this.bonesIDs.length;
  }

  constructor(fileNameVal: string) {
    if (fileNameVal.length > 0) {
      this._fileName = fileNameVal.replace(".bin", ".bytes");
    } else {
      this._fileName = "";
    }
  }

  static LoadFromBytes(bytes: Uint8Array, fileNameVal: string): AnimationBinaries {
    const result = new AnimationBinaries(fileNameVal);
    if (bytes.length === 0) {
      console.error("Animation file is empty");
      return null!;
    }

    const reader = new BinaryReaderNekki(bytes);
    const magic = reader.ReadInt64();
    if (magic !== 457546134634732) {
      console.error("Wrong animation file type");
      reader.Dispose();
      return null!;
    }

    const sectionCount = reader.ReadInt16();
    const offsets: number[] = [];
    for (let i = 0; i < sectionCount; i++) {
      offsets.push(reader.ReadInt64());
    }

    result.LoadAnimation(reader);
    if (sectionCount > 1) {
      const tangents = new AnimationBinaries("");
      tangents.LoadAnimation(reader);
      result.animationTangents = tangents;
    }

    reader.Dispose();
    return result;
  }

  private LoadAnimation(br: BinaryReaderNekki): void {
    const frameCount = br.ReadInt32();
    const boneCount = br.ReadInt32();

    this.frames = new Array(frameCount);
    this.bonesIDs = new Array(boneCount);

    for (let i = 0; i < boneCount; i++) {
      this.bonesIDs[i] = br.ReadInt16();
    }

    const rawData = br.ConvertByteArrayToFloat(frameCount * boneCount * 7);

    for (let j = 0; j < frameCount; j++) {
      const frame = new AnimationFrame();
      frame.bonesAnimation = new Array(boneCount);
      for (let k = 0; k < boneCount; k++) {
        const idx = j * boneCount * 7 + k * 7;
        const pos = new Vector3(rawData[idx], rawData[idx + 1], rawData[idx + 2]);
        const rot = new Quaternion(rawData[idx + 3], rawData[idx + 4], rawData[idx + 5], rawData[idx + 6]);
        frame.bonesAnimation[k] = new AnimatedTransform(pos, rot);
      }
      this.frames[j] = frame;
    }
  }

  CopyFrameTransformByIndex(frameNumber: number, boneIndex: number, copyTo: AnimatedTransform): void {
    if (boneIndex < 0) {
      AnimatedTransform.CopyBoneTransform(AnimatedTransform.IDENTITY, copyTo);
    } else {
      AnimatedTransform.CopyBoneTransform(this.frames[frameNumber].bonesAnimation[boneIndex], copyTo);
    }
  }

  CopyFrameTransformByID(frameNumber: number, boneID: number, copyTo: AnimatedTransform): void {
    const boneIndex = this.GetBoneIndexByID(boneID);
    if (boneIndex < 0) {
      AnimatedTransform.CopyBoneTransform(AnimatedTransform.IDENTITY, copyTo);
    } else {
      AnimatedTransform.CopyBoneTransform(this.frames[frameNumber].bonesAnimation[boneIndex], copyTo);
    }
  }

  CopyFrameTransforms(frameNumber: number, copyTo: AnimatedTransform[]): void {
    if (frameNumber >= this.frames.length) {
      throw new Error(`frameNumber ${frameNumber} is out of range frames ${this.frames.length}`);
    }
    const src = this.frames[frameNumber].bonesAnimation;
    for (let i = 0; i < src.length; i++) {
      AnimatedTransform.CopyBoneTransform(src[i], copyTo[i]);
    }
  }

  CopyFrameTransformsToDict(frameNumber: number, copyTo: Map<number, AnimatedTransform>): void {
    const src = this.frames[frameNumber].bonesAnimation;
    for (let i = 0; i < src.length; i++) {
      const boneID = this.bonesIDs[i];
      if (copyTo.has(boneID)) {
        AnimatedTransform.CopyBoneTransform(src[i], copyTo.get(boneID)!);
        copyTo.get(boneID)!.animateThisFrame = true;
      }
    }
  }

  private GetBoneIndexByID(boneID: number): number {
    for (let i = 0; i < this.bonesIDs.length; i++) {
      if (this.bonesIDs[i] === boneID) return i;
    }
    return -1;
  }
}
