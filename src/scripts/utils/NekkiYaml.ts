/**
 * NekkiYaml.ts
 *
 * Drop-in TypeScript port of the Nekki.Yaml Unity plugin.
 * Nekki.Yaml was a thin wrapper around YamlDotNet; this is the
 * equivalent thin wrapper around js-yaml.
 *
 * Mirrors the original API surface:
 *   YamlDocumentNekki  →  parse entry-point
 *   Mapping            →  YAML mapping  (object / dictionary)
 *   Sequence           →  YAML sequence (array)
 *   Scalar             →  YAML scalar   (string / number / bool)
 *
 * Install: npm install js-yaml && npm install -D @types/js-yaml
 */

import * as yaml from "js-yaml";

// ─────────────────────────────────────────────────────────────
// Node types
// ─────────────────────────────────────────────────────────────

export type YamlNode = Mapping | Sequence | Scalar;

/** Wraps a raw parsed value into the appropriate YamlNode. */
function wrapNode(value: unknown): YamlNode {
  if (Array.isArray(value))                        return new Sequence(value);
  if (value !== null && typeof value === "object") return new Mapping(value as Record<string, unknown>);
  return new Scalar(value == null ? "" : String(value));
}

// ─────────────────────────────────────────────────────────────
// Scalar  (leaf string / number / bool value)
// ─────────────────────────────────────────────────────────────

export class Scalar {
  public readonly text: string;

  constructor(value: unknown) {
    this.text = value == null ? "" : String(value);
  }

  toString(): string { return this.text; }

  toFloat(): number  { return parseFloat(this.text); }
  toInt(): number    { return parseInt(this.text, 10); }
  toBool(): boolean  { return this.text === "true" || this.text === "1"; }
}

// ─────────────────────────────────────────────────────────────
// Sequence  (array of YamlNodes)
// ─────────────────────────────────────────────────────────────

export class Sequence {
  /** Direct equivalent of C# nodesInside List<Node> */
  public readonly nodesInside: YamlNode[];

  constructor(data: unknown[]) {
    this.nodesInside = data.map(wrapNode);
  }

  get count(): number { return this.nodesInside.length; }

  /** Cast helper — mirrors (Mapping)seq.nodesInside[i] */
  getMapping(index: number): Mapping {
    const n = this.nodesInside[index];
    if (!(n instanceof Mapping)) throw new Error(`Node at [${index}] is not a Mapping`);
    return n;
  }

  getScalar(index: number): Scalar {
    const n = this.nodesInside[index];
    if (!(n instanceof Scalar)) throw new Error(`Node at [${index}] is not a Scalar`);
    return n;
  }
}

// ─────────────────────────────────────────────────────────────
// Mapping  (key → YamlNode dictionary)
// ─────────────────────────────────────────────────────────────

export class Mapping {
  private readonly _data: Record<string, unknown>;

  /**
   * nodesInside — all child nodes as a flat list, matching Unity behaviour
   * where iterating nodesInside gives every value in insertion order.
   */
  public readonly nodesInside: YamlNode[];

  constructor(data: Record<string, unknown>) {
    this._data = data;
    this.nodesInside = Object.values(data).map(wrapNode);
  }

  // ── typed getters ────────────────────────────────────────

  /** mapping.GetMapping("key")  →  child Mapping or null */
  GetMapping(key: string): Mapping | null {
    const v = this._data[key];
    if (v == null || typeof v !== "object" || Array.isArray(v)) return null;
    return new Mapping(v as Record<string, unknown>);
  }

  /** mapping.GetSequence("key")  →  child Sequence or null */
  GetSequence(key: string): Sequence | null {
    const v = this._data[key];
    if (!Array.isArray(v)) return null;
    return new Sequence(v);
  }

  /** mapping.GetText("key")  →  Scalar or null */
  GetText(key: string): Scalar | null {
    const v = this._data[key];
    if (v == null) return null;
    return new Scalar(v);
  }

  // ── raw access ───────────────────────────────────────────

  /** Get raw parsed value (useful for numbers before wrapping). */
  getRaw(key: string): unknown { return this._data[key]; }

  keys(): string[] { return Object.keys(this._data); }
  has(key: string): boolean { return Object.prototype.hasOwnProperty.call(this._data, key); }
}

// ─────────────────────────────────────────────────────────────
// YamlDocumentNekki  (parse entry-point)
// ─────────────────────────────────────────────────────────────

export class YamlDocumentNekki {
  private readonly _root: Mapping;

  private constructor(parsed: unknown) {
    if (parsed !== null && typeof parsed === "object" && !Array.isArray(parsed)) {
      this._root = new Mapping(parsed as Record<string, unknown>);
    } else {
      // Edge-case: top-level is a sequence or scalar — wrap it.
      this._root = new Mapping({ _root: parsed });
    }
  }

  // ── static constructors ──────────────────────────────────

  /** YamlDocumentNekki.FromYamlContent(text) */
  static FromYamlContent(content: string): YamlDocumentNekki {
    const parsed = yaml.load(content);
    return new YamlDocumentNekki(parsed);
  }

  /** Load from a URL / asset path (async, browser-friendly). */
  static async FromUrl(url: string): Promise<YamlDocumentNekki> {
    const res = await fetch(url);
    if (!res.ok) throw new Error(`NekkiYaml: failed to fetch ${url} (${res.status})`);
    const text = await res.text();
    return YamlDocumentNekki.FromYamlContent(text);
  }

  // ── instance ─────────────────────────────────────────────

  /** doc.GetRoot() */
  GetRoot(): Mapping { return this._root; }

  /**
   * SaveToFile — not applicable in a browser/Node context.
   * Logs a warning and returns the YAML string instead.
   */
  SaveToFile(_path: string, _overwrite: boolean): string {
    console.warn("YamlDocumentNekki.SaveToFile: file I/O not available in this environment.");
    return yaml.dump(this._toPlain(this._root));
  }

  /** Dump back to a YAML string (utility). */
  toString(): string {
    return yaml.dump(this._toPlain(this._root));
  }

  // ── private helpers ──────────────────────────────────────

  private _toPlain(node: YamlNode): unknown {
    if (node instanceof Scalar)   return node.text;
    if (node instanceof Sequence) return node.nodesInside.map(n => this._toPlain(n));
    // Mapping
    const obj: Record<string, unknown> = {};
    for (const k of (node as Mapping).keys()) {
      const child = (node as Mapping).GetText(k) ?? (node as Mapping).GetMapping(k) ?? (node as Mapping).GetSequence(k);
      if (child) obj[k] = this._toPlain(child as YamlNode);
    }
    return obj;
  }
}
