export class BinaryReaderNekki {
  public readonly Size: number;
  public Position: number = 0;
  private _bytes: Uint8Array;

  constructor(data: Uint8Array) {
    this._bytes = data;
    this.Size = data.length;
  }

  ReadBoolean(): boolean {
    const result = !!this._bytes[this.Position];
    this.Position += 1;
    return result;
  }

  ReadByte(): number {
    const result = this._bytes[this.Position];
    this.Position += 1;
    return result;
  }

  ReadInt16(): number {
    const view = new DataView(this._bytes.buffer, this._bytes.byteOffset + this.Position, 2);
    const result = view.getInt16(0, true);
    this.Position += 2;
    return result;
  }

  ReadInt32(): number {
    const view = new DataView(this._bytes.buffer, this._bytes.byteOffset + this.Position, 4);
    const result = view.getInt32(0, true);
    this.Position += 4;
    return result;
  }

  ReadInt64(): number {
    const view = new DataView(this._bytes.buffer, this._bytes.byteOffset + this.Position, 8);
    const result = Number(view.getBigInt64(0, true));
    this.Position += 8;
    return result;
  }

  ReadSingle(): number {
    const view = new DataView(this._bytes.buffer, this._bytes.byteOffset + this.Position, 4);
    const result = view.getFloat32(0, true);
    this.Position += 4;
    return result;
  }

  ConvertByteArrayToFloat(count: number): Float32Array {
    const num = count * 4;
    const result = new Float32Array(count);
    for (let i = 0; i < count; i++) {
      result[i] = new DataView(this._bytes.buffer, this._bytes.byteOffset + this.Position + i * 4, 4).getFloat32(0, true);
    }
    this.Position += num;
    return result;
  }

  Dispose(): void {
    this._bytes = new Uint8Array(0);
  }
}
