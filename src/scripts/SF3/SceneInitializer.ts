import { Vector3, FreeCamera, HemisphericLight, DirectionalLight, Scene, Bone, Quaternion } from "@babylonjs/core";
import "@babylonjs/loaders";
import { SceneManager } from "../core/SceneManager";
import { Gender } from "../sf3DTO/Gender";
import { EquipmentType } from "./Items/EquipmentType";
import { assembleCharacter } from "./GameModels/ModelComponents";

// Global positions and rotations straight from Unity skeleton dump
const BIND_POSE: Record<string, { pos: [number,number,number], rot: [number,number,number,number] }> = {
  "pelvis":              { pos: [-105.750,  80.938,  -7.070], rot: [ 0.0196, -0.2812, -0.0333,  0.9589] },
  "zero_joint_pelvis_l": { pos: [-105.750,  80.938,  -7.070], rot: [ 0.0196, -0.2812, -0.0333,  0.9589] },
  "thigh_l":             { pos: [-114.096,  82.097, -10.787], rot: [ 0.0530, -0.6905, -0.0457,  0.7199] },
  "calf_l":              { pos: [-148.211,  49.767,   3.287], rot: [ 0.0499,  0.5731,  0.0009,  0.8180] },
  "foot_l":              { pos: [-152.879,  12.394, -13.223], rot: [ 0.0001, -0.3406, -0.0001,  0.9402] },
  "toe_l":               { pos: [-165.312,   2.301,  -0.156], rot: [-0.0032, -0.3406, -0.0013,  0.9402] },
  "thigh_twist_l":       { pos: [-126.585,  69.701,  -6.025], rot: [-0.0305,  0.8869, -0.0166, -0.4607] },
  "back_l":              { pos: [-109.283,  77.661, -19.888], rot: [ 0.0238, -0.1512, -0.0304,  0.9878] },
  "stomach":             { pos: [-105.452,  91.816,  -5.262], rot: [-0.0167, -0.3092, -0.0043,  0.9508] },
  "chest":               { pos: [-104.960, 109.559,  -5.274], rot: [-0.0174, -0.2968,  0.0531,  0.9533] },
  "zero_joint_hand_l":   { pos: [-104.960, 109.559,  -5.274], rot: [-0.0174, -0.2968,  0.0531,  0.9533] },
  "chest_l":             { pos: [-123.178, 114.886,   4.693], rot: [-0.0174, -0.2968,  0.0531,  0.9533] },
  "clavicle_l":          { pos: [-111.292, 133.672,  -3.881], rot: [ 0.0262, -0.3800,  0.0353,  0.9240] },
  "arm_l":               { pos: [-124.895, 127.953, -14.988], rot: [-0.0359, -0.7193,  0.0311,  0.6930] },
  "biceps_twist_l":      { pos: [-129.133, 115.620, -19.839], rot: [ 0.0489, -0.4303,  0.0512,  0.8999] },
  "forearm_l":           { pos: [-133.711, 102.290, -25.084], rot: [-0.0730,  0.6346, -0.1745,  0.7493] },
  "hand_l":              { pos: [-151.939,  93.721, -10.975], rot: [-0.3415,  0.1696, -0.1750,  0.9077] },
  "f_big1_l":            { pos: [-155.718,  97.132,  -7.692], rot: [-0.4253,  0.1070, -0.0275,  0.8983] },
  "f_big2_l":            { pos: [-157.912,  97.698,  -4.502], rot: [-0.0922, -0.2801,  0.2271,  0.9282] },
  "f_big3_l":            { pos: [-159.856,  94.621,  -2.271], rot: [ 0.4373, -0.3483,  0.4819,  0.6747] },
  "f_pointer1_l":        { pos: [-163.386,  95.563,  -8.128], rot: [-0.4550,  0.6761, -0.1906,  0.5473] },
  "f_pointer2_l":        { pos: [-163.214,  94.173,  -3.901], rot: [-0.1801, -0.2028,  0.5427,  0.7949] },
  "f_pointer3_l":        { pos: [-159.925,  95.151,  -3.850], rot: [-0.2902,  0.1603,  0.5199,  0.7872] },
  "weapon_l":            { pos: [-159.918,  92.167,  -6.401], rot: [-0.3776, -0.2048, -0.0341,  0.9024] },
  "f_main1_l":           { pos: [-162.706,  90.379,  -9.055], rot: [-0.1470,  0.8267, -0.5047, -0.2005] },
  "f_main2_l":           { pos: [-160.501,  90.364,  -4.597], rot: [-0.3344, -0.2478,  0.4239,  0.8044] },
  "f_main3_l":           { pos: [-156.227,  92.275,  -5.487], rot: [-0.2708,  0.2120,  0.7529,  0.5612] },
  "forearm_twist_l":     { pos: [-142.555,  98.127, -18.233], rot: [-0.2090,  0.6352, -0.2051,  0.7146] },
  "scapular_l":          { pos: [-123.820, 135.762, -13.573], rot: [ 0.0315, -0.5236,  0.0307,  0.8508] },
  "zero_joint_hand_r":   { pos: [-104.960, 109.558,  -5.274], rot: [-0.0229, -0.1942,  0.0510,  0.9794] },
  "clavicle_r":          { pos: [-107.850, 134.073,  -1.510], rot: [-0.1124,  0.8897,  0.2182,  0.3851] },
  "arm_r":               { pos: [ -94.099, 133.060,  10.778], rot: [ 0.0140,  0.6780,  0.0231,  0.7345] },
  "biceps_twist_r":      { pos: [ -85.253, 122.926,  14.328], rot: [-0.0157,  0.8573,  0.2003,  0.4740] },
  "forearm_r":           { pos: [ -75.690, 111.971,  18.161], rot: [-0.0542,  0.6369,  0.1581,  0.7527] },
  "hand_r":              { pos: [ -97.553, 103.356,  25.405], rot: [-0.1796,  0.7584,  0.0593,  0.6237] },
  "weapon_r":            { pos: [-106.350, 100.134,  24.666], rot: [-0.1882,  0.9609, -0.0300,  0.2009] },
  "f_pointer1_r":        { pos: [-109.469, 103.124,  27.507], rot: [-0.1277,  0.2454,  0.2590,  0.9254] },
  "f_pointer2_r":        { pos: [-110.761, 101.110,  23.603], rot: [ 0.1165, -0.4460,  0.2506,  0.8513] },
  "f_pointer3_r":        { pos: [-108.026, 102.616,  22.071], rot: [-0.2326,  0.8326, -0.1492, -0.4800] },
  "f_big1_r":            { pos: [-103.074, 105.795,  23.680], rot: [-0.0521, -0.2608,  0.3611,  0.8938] },
  "f_big2_r":            { pos: [-106.650, 105.385,  21.638], rot: [-0.0098, -0.2876,  0.4809,  0.8282] },
  "f_big3_r":            { pos: [-108.705, 101.246,  20.846], rot: [-0.1364, -0.1250,  0.9134,  0.3627] },
  "f_main1_r":           { pos: [-107.275,  98.379,  28.389], rot: [-0.0124,  0.0000,  0.2680,  0.9633] },
  "f_main2_r":           { pos: [-107.160,  98.038,  23.672], rot: [ 0.1957, -0.5238,  0.1873,  0.8076] },
  "f_main3_r":           { pos: [-103.682, 100.473,  22.366], rot: [-0.3849,  0.9121, -0.0674, -0.1245] },
  "forearm_twist_r":     { pos: [ -86.304, 107.789,  21.672], rot: [-0.1882,  0.8120,  0.0961,  0.5441] },
  "scapular_r":          { pos: [ -98.689, 139.613,  10.348], rot: [-0.1059,  0.8780,  0.2213,  0.4110] },
  "chest_r":             { pos: [-108.722, 116.571,  14.634], rot: [-0.0229, -0.1942,  0.0510,  0.9794] },
  "neck":                { pos: [-109.693, 138.257,  -3.259], rot: [ 0.0133, -0.3673,  0.0633,  0.9279] },
  "head":                { pos: [-114.410, 150.565,   0.486], rot: [-0.0088, -0.6652, -0.0368,  0.7457] },
  "hair":                { pos: [-111.927, 165.771,   1.155], rot: [-0.0088, -0.6652, -0.0368,  0.7457] },
  "zero_joint_pelvis_r": { pos: [-105.750,  80.937,  -7.070], rot: [ 0.0295,  0.0545, -0.0249,  0.9978] },
  "thigh_r":             { pos: [ -98.825,  80.061,  -1.062], rot: [-0.2016, -0.0074, -0.0294,  0.9790] },
  "thigh_twist_r":       { pos: [ -91.769,  65.774,   7.794], rot: [-0.1614, -0.1410,  0.0105,  0.9767] },
  "calf_r":              { pos: [ -79.871,  42.520,  24.212], rot: [-0.0734,  0.2060, -0.0685,  0.9734] },
  "foot_r":              { pos: [ -78.046,  12.250,  -3.562], rot: [-0.0060, -0.8463,  0.0119,  0.5326] },
  "toe_r":               { pos: [ -70.829,   2.644,  13.255], rot: [ 0.0453,  0.6014,  0.0107,  0.7976] },
  "back_r":              { pos: [ -93.463,  75.551,  -9.812], rot: [ 0.0335,  0.9567,  0.0193,  0.2886] },
};

export async function initializeScene(mgr: SceneManager): Promise<void> {
  const scene = mgr.scene;

  const cam = new FreeCamera("cam", new Vector3(0, 155, -950), scene);
  cam.setTarget(Vector3.Zero());
  cam.minZ = 10; cam.maxZ = 5000;
  cam.fov = 30 * Math.PI / 180;
  scene.activeCamera = cam;

  new HemisphericLight("hemi", new Vector3(0, 1, 0), scene);
  const dir = new DirectionalLight("dir", new Vector3(-0.5, -1, -0.3), scene);
  dir.position.set(300, 500, 300);

  const result = await assembleCharacter(scene, Gender.Male, "head__01a", [
    { type: EquipmentType.Armor, model: "arm__base" },
    { type: EquipmentType.Helmet, model: "hair-01" },
  ]);

  const boneByName = new Map<string, Bone>();
  for (const sk of scene.skeletons) {
    for (const b of sk.bones) {
      if (!boneByName.has(b.name)) boneByName.set(b.name, b);
    }
  }

  // Set global position and rotation directly on each bone's transform node
  for (const [boneName, data] of Object.entries(BIND_POSE)) {
    const bone = boneByName.get(boneName);
    if (!bone) continue;
    const tn = bone.getTransformNode();
    if (!tn) continue;
    tn.setAbsolutePosition(new Vector3(data.pos[0], data.pos[1], data.pos[2]));
    tn.rotationQuaternion = new Quaternion(data.rot[0], data.rot[1], data.rot[2], data.rot[3]);
  }

  for (const sk of scene.skeletons) {
    (sk as any)._isDirty = true;
    (sk as any).prepare(true);
  }

  console.log("Bind pose applied to", Object.keys(BIND_POSE).length, "bones");
}
