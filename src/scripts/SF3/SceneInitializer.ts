import { Vector3, FreeCamera, HemisphericLight, DirectionalLight, Scene, Bone, Quaternion } from "@babylonjs/core";
import "@babylonjs/loaders";
import { SceneManager } from "../core/SceneManager";
import { Gender } from "../sf3DTO/Gender";
import { EquipmentType } from "./Items/EquipmentType";
import { assembleCharacter } from "./GameModels/ModelComponents";

// Hardcoded bind pose — global rotations from Unity skeleton dump
// Format: boneName -> [x, y, z, w]
const BIND_POSE: Record<string, [number,number,number,number]> = {
  "pelvis":               [ 0.0196, -0.2812, -0.0333,  0.9589],
  "zero_joint_pelvis_l":  [ 0.0196, -0.2812, -0.0333,  0.9589],
  "thigh_l":              [ 0.0530, -0.6905, -0.0457,  0.7199],
  "calf_l":               [ 0.0499,  0.5731,  0.0009,  0.8180],
  "foot_l":               [ 0.0001, -0.3406, -0.0001,  0.9402],
  "toe_l":                [-0.0032, -0.3406, -0.0013,  0.9402],
  "thigh_twist_l":        [-0.0305,  0.8869, -0.0166, -0.4607],
  "back_l":               [ 0.0238, -0.1512, -0.0304,  0.9878],
  "stomach":              [-0.0167, -0.3092, -0.0043,  0.9508],
  "chest":                [-0.0174, -0.2968,  0.0531,  0.9533],
  "zero_joint_hand_l":    [-0.0174, -0.2968,  0.0531,  0.9533],
  "chest_l":              [-0.0174, -0.2968,  0.0531,  0.9533],
  "clavicle_l":           [ 0.0262, -0.3800,  0.0353,  0.9240],
  "arm_l":                [-0.0359, -0.7193,  0.0311,  0.6930],
  "biceps_twist_l":       [ 0.0489, -0.4303,  0.0512,  0.8999],
  "forearm_l":            [-0.0730,  0.6346, -0.1745,  0.7493],
  "hand_l":               [-0.3415,  0.1696, -0.1750,  0.9077],
  "f_big1_l":             [-0.4253,  0.1070, -0.0275,  0.8983],
  "f_big2_l":             [-0.0922, -0.2801,  0.2271,  0.9282],
  "f_big3_l":             [ 0.4373, -0.3483,  0.4819,  0.6747],
  "f_pointer1_l":         [-0.4550,  0.6761, -0.1906,  0.5473],
  "f_pointer2_l":         [-0.1801, -0.2028,  0.5427,  0.7949],
  "f_pointer3_l":         [-0.2902,  0.1603,  0.5199,  0.7872],
  "weapon_l":             [-0.3776, -0.2048, -0.0341,  0.9024],
  "f_main1_l":            [-0.1470,  0.8267, -0.5047, -0.2005],
  "f_main2_l":            [-0.3344, -0.2478,  0.4239,  0.8044],
  "f_main3_l":            [-0.2708,  0.2120,  0.7529,  0.5612],
  "forearm_twist_l":      [-0.2090,  0.6352, -0.2051,  0.7146],
  "scapular_l":           [ 0.0315, -0.5236,  0.0307,  0.8508],
  "zero_joint_hand_r":    [-0.0229, -0.1942,  0.0510,  0.9794],
  "clavicle_r":           [-0.1124,  0.8897,  0.2182,  0.3851],
  "arm_r":                [ 0.0140,  0.6780,  0.0231,  0.7345],
  "biceps_twist_r":       [-0.0157,  0.8573,  0.2003,  0.4740],
  "forearm_r":            [-0.0542,  0.6369,  0.1581,  0.7527],
  "hand_r":               [-0.1796,  0.7584,  0.0593,  0.6237],
  "weapon_r":             [-0.1882,  0.9609, -0.0300,  0.2009],
  "f_pointer1_r":         [-0.1277,  0.2454,  0.2590,  0.9254],
  "f_pointer2_r":         [ 0.1165, -0.4460,  0.2506,  0.8513],
  "f_pointer3_r":         [-0.2326,  0.8326, -0.1492, -0.4800],
  "f_big1_r":             [-0.0521, -0.2608,  0.3611,  0.8938],
  "f_big2_r":             [-0.0098, -0.2876,  0.4809,  0.8282],
  "f_big3_r":             [-0.1364, -0.1250,  0.9134,  0.3627],
  "f_main1_r":            [-0.0124, -0.0000,  0.2680,  0.9633],
  "f_main2_r":            [ 0.1957, -0.5238,  0.1873,  0.8076],
  "f_main3_r":            [-0.3849,  0.9121, -0.0674, -0.1245],
  "forearm_twist_r":      [-0.1882,  0.8120,  0.0961,  0.5441],
  "scapular_r":           [-0.1059,  0.8780,  0.2213,  0.4110],
  "chest_r":              [-0.0229, -0.1942,  0.0510,  0.9794],
  "neck":                 [ 0.0133, -0.3673,  0.0633,  0.9279],
  "head":                 [-0.0088, -0.6652, -0.0368,  0.7457],
  "hair":                 [-0.0088, -0.6652, -0.0368,  0.7457],
  "zero_joint_pelvis_r":  [ 0.0295,  0.0545, -0.0249,  0.9978],
  "thigh_r":              [-0.2016, -0.0074, -0.0294,  0.9790],
  "thigh_twist_r":        [-0.1614, -0.1410,  0.0105,  0.9767],
  "calf_r":               [-0.0734,  0.2060, -0.0685,  0.9734],
  "foot_r":               [-0.0060, -0.8463,  0.0119,  0.5326],
  "toe_r":                [ 0.0453,  0.6014,  0.0107,  0.7976],
  "back_r":               [ 0.0335,  0.9567,  0.0193,  0.2886],
};

export async function initializeScene(mgr: SceneManager): Promise<void> {
  const scene = mgr.scene;

  // Camera
  const cam = new FreeCamera("cam", new Vector3(0, 155, -950), scene);
  cam.setTarget(Vector3.Zero());
  cam.minZ = 10; cam.maxZ = 5000;
  cam.fov = 30 * Math.PI / 180;
  scene.activeCamera = cam;

  // Lights
  new HemisphericLight("hemi", new Vector3(0, 1, 0), scene);
  const dir = new DirectionalLight("dir", new Vector3(-0.5, -1, -0.3), scene);
  dir.position.set(300, 500, 300);

  const result = await assembleCharacter(scene, Gender.Male, "head__01a", [
    { type: EquipmentType.Armor, model: "arm__base" },
    { type: EquipmentType.Helmet, model: "hair-01" },
  ]);

  // Build bone map
  const boneByName = new Map<string, Bone>();
  for (const sk of scene.skeletons) {
    for (const b of sk.bones) {
      if (!boneByName.has(b.name)) boneByName.set(b.name, b);
    }
  }

  // Slam the hardcoded global rotations directly onto every bone's TN
  for (const [boneName, q] of Object.entries(BIND_POSE)) {
    const bone = boneByName.get(boneName);
    if (!bone) continue;
    const tn = bone.getTransformNode();
    if (!tn) continue;
    tn.rotationQuaternion = new Quaternion(q[0], q[1], q[2], q[3]);
  }

  // Force recompute
  for (const sk of scene.skeletons) {
    (sk as any)._isDirty = true;
    (sk as any).prepare(true);
  }

  console.log("Bind pose applied. Bones set:", Object.keys(BIND_POSE).length);
}
