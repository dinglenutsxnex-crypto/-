using System;
using MKGlowSystem;
using SF3.Audio;
using SF3.GameModels;
using UnityEngine;

namespace SF3
{
	public class BattleCamera : MonoBehaviour, ISceneInitializationObject
	{
		public enum ECameraState
		{
			BATTLE = 0,
			INVENTORY = 1,
			MOVE_TO_POINT = 2,
			MOVE_TO_OBJECT = 3,
			STAY = 4
		}

		public enum AspectRatio
		{
			_4X3 = 0,
			_16X9 = 1
		}

		[Serializable]
		public class CameraSettings
		{
			public AspectRatio aspect;

			public float camZOffset;

			public float defaultScreenRatio;

			public float YOffsetDuringZMove;
		}

		private static BattleCamera _instance;

		public CameraSettings[] settings;

		public Vector3 mapCameraPosition;

		public Vector3 boosterpackCameraPosition;

		public string switchCameraTargetSoundName = string.Empty;

		private Vector3 _startCameraPosition;

		public bool lookAtPlayerPos;

		public Vector3 lookAtPointOffset;

		private Model player;

		private Model enemy;

		public float moveSmoothTime;

		public float rotatioSmoothTime;

		private float dist;

		private float cameraDistRatio;

		private float _minDist;

		[SerializeField]
		private float _maxDistance = 500f;

		[SerializeField]
		[Range(0f, 1f)]
		private float screenShootDistance;

		public float currentMaxDistance;

		public float FOV;

		public float centerMassK;

		public AnimationCurve YZMove;

		private float YOffsetDuringZMove;

		private float charsZ;

		private float YZoffset;

		private float defaultScreenRatio;

		public bool pauseOnMove;

		private float startMoveTime;

		public AnimationCurve moveToTargetCurve;

		public float moveTime;

		private float moveTimer;

		private Quaternion startRot;

		private Quaternion endRotation;

		private Vector3 startPos;

		private Vector3 endPos;

		public float mapClippingPlane = 290f;

		private Transform _targetTransform;

		[HideInInspector]
		public float fromClipPlane;

		[HideInInspector]
		public float toClipPlane;

		[HideInInspector]
		public bool changeClipPlane;

		public bool onlyPlayer;

		public const float DEF_CLIPPING_PLANE = 10f;

		private const float DEF_MIN_DIST = 350f;

		private float defOffsetFromChar;

		public float moveThreshold;

		private float zOffset;

		private Transform _transform;

		private Vector3 calculatedPosition = default(Vector3);

		private Vector3 currVelocity;

		private Vector3 rotationVelocity;

		private float xVelocity;

		private float yVelocity;

		private float zVelocity;

		private Vector3 targetRot;

		private float halfScreenWidth;

		private float startFollowYUp;

		private float startFollowYBot;

		private float leftHorizontalAngle;

		private float rightHorizontalAngle;

		private float maxYPosition;

		private float minYPosition;

		private float maxXPosition;

		private float minXPosition;

		private float upVerticalAngle;

		private float botVerticalAngle;

		private float upRotationBorder;

		private float botRotationBorder;

		private Vector3 camOffset = new Vector3(0f, -98.5f, 562.6f);

		[SerializeField]
		private Vector3 _defaultPosition = new Vector3(0f, 155f, -950f);

		private Vector3? _startPosition;

		private float middleXBetweenModels;

		public Vector3 fixedOffset;

		private ECameraState _cameraState = ECameraState.STAY;

		private bool bordersActive;

		private const float widthRatioForBorder = 0.55f;

		private float playerCenterMassY;

		private float enemyCenterMassY;

		private static bool ignoreOnDone;

		public static BattleCamera Instance
		{
			get
			{
				return _instance;
			}
		}

		public static CameraSettings CurrentSettings { get; private set; }

		public Camera MainCamera { get; private set; }

		private bool BlockCameraMovement { get; set; }

		private bool CameraMotion { get; set; }

		private static event Action _onDone;

		private void Awake()
		{
			_instance = this;
			_transform = base.transform;
			MainCamera = Camera.main;
		}

		public void Initialize()
		{
			fixedOffset = Vector3.zero;
			_cameraState = ECameraState.STAY;
			if (MainCamera.aspect >= 1.5f)
			{
				CurrentSettings = GetSettingsByAspect(AspectRatio._16X9);
			}
			else
			{
				CurrentSettings = GetSettingsByAspect(AspectRatio._4X3);
			}
			ReadConfigData();
			FixForResolution();
			_startCameraPosition = SceneConfig.SpawnPointPlayer - _instance.camOffset;
			_startCameraPosition.x = 0f;
			charsZ = SceneConfig.SpawnPointPlayer.z;
			defOffsetFromChar = _instance.charsZ - _instance._startCameraPosition.z;
			if (_minDist > currentMaxDistance)
			{
				float minDist = _minDist;
				_minDist = currentMaxDistance;
				currentMaxDistance = minDist;
			}
			MKGlow[] array = UnityEngine.Object.FindObjectsOfType<MKGlow>();
			MKGlow[] array2 = array;
			foreach (MKGlow mKGlow in array2)
			{
				if (mKGlow != null)
				{
					mKGlow.enabled = false;
				}
			}
			if (switchCameraTargetSoundName.Length > 0)
			{
				AudioManager.Instance.LoadSound(switchCameraTargetSoundName);
			}
			MoveToDefault(true);
		}

		public void DisposePreviousLocation()
		{
			_cameraState = ECameraState.STAY;
		}

		public static void SetModels(Model playerValue, Model enemyValue)
		{
			_instance.player = playerValue;
			_instance.enemy = enemyValue;
			if (_instance._cameraState != ECameraState.STAY)
			{
				MoveToDojo(null, true);
			}
		}

		private void FixForResolution()
		{
			float num = (float)Screen.width / (float)Screen.height / defaultScreenRatio;
			_minDist = 350f * num;
			currentMaxDistance = _maxDistance;
		}

		private void FixNaNValues(ref Vector3 targetRot)
		{
			if (float.IsNaN(targetRot.z))
			{
				targetRot.z = _transform.eulerAngles.z;
			}
			if (float.IsNaN(targetRot.x))
			{
				targetRot.x = _transform.eulerAngles.x;
			}
			if (float.IsNaN(targetRot.y))
			{
				targetRot.y = _transform.eulerAngles.y;
			}
		}

		private float ClampAngle(float source, float minAngle, float maxAngle)
		{
			if (source < 0f)
			{
				source += 360f;
			}
			else if (source > 360f)
			{
				source -= 360f;
			}
			if (source < 180f)
			{
				if (minAngle == 0f)
				{
					source = 0f;
				}
				else if (source >= minAngle)
				{
					source = minAngle;
				}
			}
			else if (maxAngle == 0f)
			{
				source = 0f;
			}
			else if (source <= 360f - maxAngle)
			{
				source = 360f - maxAngle;
			}
			return source;
		}

		private void ClampRotation(ref Vector3 targetRot)
		{
			targetRot.y = ClampAngle(targetRot.y, rightHorizontalAngle, leftHorizontalAngle);
			targetRot.x = ClampAngle(targetRot.x, botVerticalAngle, upVerticalAngle);
		}

		private void CheckVerticalRotationRange()
		{
			if (playerCenterMassY >= upRotationBorder || enemyCenterMassY >= upRotationBorder)
			{
				calculatedPosition.y = _startCameraPosition.y + (((!(playerCenterMassY > enemyCenterMassY)) ? enemyCenterMassY : playerCenterMassY) - _startCameraPosition.y) * centerMassK;
			}
			else if (playerCenterMassY <= botRotationBorder || enemyCenterMassY <= botRotationBorder)
			{
				calculatedPosition.y = _startCameraPosition.y + (((!(playerCenterMassY < enemyCenterMassY)) ? enemyCenterMassY : playerCenterMassY) - _startCameraPosition.y) * centerMassK;
			}
			else
			{
				calculatedPosition.y = _startCameraPosition.y;
			}
		}

		private void CheckVerticalMovementDiapason()
		{
			if (playerCenterMassY >= startFollowYUp || enemyCenterMassY >= startFollowYUp)
			{
				calculatedPosition.y = _startCameraPosition.y + (((!(playerCenterMassY > enemyCenterMassY)) ? enemyCenterMassY : playerCenterMassY) - _startCameraPosition.y) * centerMassK;
			}
			else if (playerCenterMassY <= startFollowYBot || enemyCenterMassY <= startFollowYBot)
			{
				calculatedPosition.y = _startCameraPosition.y + (((!(playerCenterMassY < enemyCenterMassY)) ? enemyCenterMassY : playerCenterMassY) - _startCameraPosition.y) * centerMassK;
			}
			else
			{
				calculatedPosition.y = _startCameraPosition.y;
			}
			calculatedPosition.y = Mathf.Clamp(calculatedPosition.y, minYPosition, maxYPosition);
		}

		internal void LateUpdate()
		{
			if (_cameraState == ECameraState.BATTLE)
			{
				if (GameTimeController.timeScale != 0f && !BlockCameraMovement)
				{
					CalculateBattleCameraPosition();
					_transform.position = Vector3.SmoothDamp(_transform.position, calculatedPosition, ref currVelocity, moveSmoothTime);
				}
			}
			else if (_cameraState == ECameraState.INVENTORY)
			{
				calculatedPosition.x = player.modelComponents.centerOfMassBone.position.x;
				calculatedPosition.y = player.modelComponents.centerOfMassBone.position.y;
				calculatedPosition.z = _transform.position.z;
				calculatedPosition += fixedOffset;
				_transform.position = Vector3.SmoothDamp(_transform.position, calculatedPosition, ref currVelocity, 0.1f);
			}
			else if (_cameraState == ECameraState.MOVE_TO_POINT)
			{
				MoveToTarget();
			}
			else if (_cameraState == ECameraState.MOVE_TO_OBJECT)
			{
				_instance.endPos = _targetTransform.position + fixedOffset;
				MoveToTarget();
			}
		}

		private void CalculateBattleCameraPosition()
		{
			if (onlyPlayer)
			{
				calculatedPosition.x = player.modelCollisions.modelBackPos;
			}
			else
			{
				middleXBetweenModels = (calculatedPosition.x = (enemy.modelCollisions.modelBackPos + player.modelCollisions.modelBackPos) / 2f);
				if (bordersActive)
				{
					dist = Mathf.Clamp(Math.Abs(enemy.modelCollisions.modelBackPos - player.modelCollisions.modelBackPos), _minDist, currentMaxDistance);
					if (dist < currentMaxDistance)
					{
						bordersActive = false;
					}
					else
					{
						calculatedPosition.x = (SceneConfig.LeftBorderX + SceneConfig.RightBorderX) / 2f;
					}
				}
			}
			playerCenterMassY = ModelsManager.Instance.Player.modelComponents.centerOfMassBone.position.y;
			enemyCenterMassY = ModelsManager.Instance.Enemy.modelComponents.centerOfMassBone.position.y;
			YZoffset = Mathf.Lerp(0f, YOffsetDuringZMove, YZMove.Evaluate((dist - _minDist) / (currentMaxDistance - _minDist)));
			CheckVerticalRotationRange();
			calculatedPosition.z = _transform.position.z;
			if (lookAtPlayerPos)
			{
				lookAtPointOffset.z = charsZ - _transform.position.z;
				Vector3 vector = _transform.position - new Vector3(0f, YZoffset, 0f);
				targetRot = Quaternion.LookRotation(calculatedPosition + lookAtPointOffset - vector).eulerAngles;
				ClampRotation(ref targetRot);
				targetRot.x = Mathf.SmoothDampAngle(_transform.eulerAngles.x, targetRot.x, ref xVelocity, rotatioSmoothTime);
				targetRot.y = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetRot.y, ref yVelocity, rotatioSmoothTime);
				targetRot.z = Mathf.SmoothDampAngle(_transform.eulerAngles.z, targetRot.z, ref zVelocity, rotatioSmoothTime);
				FixNaNValues(ref targetRot);
				_transform.eulerAngles = targetRot;
			}
			calculatedPosition.x = Mathf.Clamp(calculatedPosition.x, minXPosition, maxXPosition);
			CheckVerticalMovementDiapason();
			if (!onlyPlayer)
			{
				if (bordersActive)
				{
					calculatedPosition.z = charsZ - zOffset;
				}
				else
				{
					dist = Mathf.Clamp(Math.Abs(enemy.modelCollisions.modelBackPos - player.modelCollisions.modelBackPos), _minDist, currentMaxDistance);
					zOffset = dist / _minDist * defOffsetFromChar;
					calculatedPosition.z = charsZ - zOffset;
					if (dist == currentMaxDistance)
					{
						bordersActive = true;
						SceneConfig.LeftBorderX = Mathf.Clamp(middleXBetweenModels - currentMaxDistance * 0.55f, SceneConfig.LocationLeftBorder, SceneConfig.LocationRightBorder);
						SceneConfig.RightBorderX = Mathf.Clamp(middleXBetweenModels + currentMaxDistance * 0.55f, SceneConfig.LocationLeftBorder, SceneConfig.LocationRightBorder);
					}
					else
					{
						SceneConfig.LeftBorderX = SceneConfig.LocationLeftBorder;
						SceneConfig.RightBorderX = SceneConfig.LocationRightBorder;
					}
				}
				calculatedPosition.y += YZoffset;
			}
			else
			{
				calculatedPosition.z = _startCameraPosition.z;
			}
			calculatedPosition += fixedOffset;
			Vector3? startPosition = _startPosition;
			if (!startPosition.HasValue)
			{
				_startPosition = calculatedPosition;
			}
		}

		public static void ActivateInventoryCamera(Vector3 offsetFromChar)
		{
			_instance._cameraState = ECameraState.INVENTORY;
			_instance.onlyPlayer = true;
			_instance.fixedOffset.x = offsetFromChar.x;
			_instance.fixedOffset.y = offsetFromChar.y;
			_instance.fixedOffset.z = 0f;
			_instance.lookAtPlayerPos = false;
			_instance.transform.localRotation = Quaternion.identity;
		}

		public void ActivateBattleCamera()
		{
			ActivateBattleCamera(false);
		}

		public void ActivateBattleCamera(bool instantly)
		{
			_instance._cameraState = ECameraState.BATTLE;
			_instance._startPosition = null;
			_instance.onlyPlayer = false;
			_instance.fixedOffset.x = 0f;
			_instance.fixedOffset.y = 0f;
			_instance.fixedOffset.z = 0f;
			_instance.lookAtPlayerPos = true;
			calculatedPosition = Vector3.zero;
			if (instantly)
			{
				CalculateBattleCameraPosition();
				_transform.position = calculatedPosition;
			}
		}

		private void MoveToTarget()
		{
			if (pauseOnMove)
			{
				moveTimer = Time.realtimeSinceStartup - startMoveTime;
			}
			else
			{
				moveTimer += GameTimeController.unscaledDeltaTime;
			}
			float t = moveToTargetCurve.Evaluate(moveTimer / moveTime);
			_transform.position = Vector3.Lerp(startPos, endPos, t);
			_transform.rotation = Quaternion.Lerp(startRot, endRotation, t);
			if (changeClipPlane)
			{
				MainCamera.nearClipPlane = Mathf.Lerp(fromClipPlane, toClipPlane, t);
			}
			if (moveTimer >= moveTime)
			{
				if (changeClipPlane)
				{
					changeClipPlane = false;
				}
				_cameraState = ECameraState.STAY;
				MoveDone();
			}
		}

		private void ReadConfigData()
		{
			MainCamera.farClipPlane = CameraConfiguration.Current.farClipPlane;
			maxYPosition = CameraConfiguration.Current.maxYPosition;
			minYPosition = CameraConfiguration.Current.minYPosition;
			maxXPosition = CameraConfiguration.Current.maxXPosition;
			minXPosition = CameraConfiguration.Current.minXPosition;
			botRotationBorder = CameraConfiguration.Current.startRotateYBot;
			upRotationBorder = CameraConfiguration.Current.startRotateYUp;
			startFollowYBot = CameraConfiguration.Current.startFollowYBot;
			startFollowYUp = CameraConfiguration.Current.startFollowYUp;
			leftHorizontalAngle = CameraConfiguration.Current.leftHorizontalAngle;
			rightHorizontalAngle = CameraConfiguration.Current.rightHorizontalAngle;
			botVerticalAngle = CameraConfiguration.Current.botVerticalAngle;
			upVerticalAngle = CameraConfiguration.Current.upVerticalAngle;
			camOffset.z = CurrentSettings.camZOffset;
			defaultScreenRatio = CurrentSettings.defaultScreenRatio;
			YOffsetDuringZMove = CurrentSettings.YOffsetDuringZMove;
			MainCamera.fieldOfView = FOV;
		}

		public static void MoveByOffset(Vector3 offset, Action onDone, bool instant, bool playSound)
		{
			Move(_instance._transform.position + offset, onDone, instant, playSound);
		}

		public static void Move(Vector3 targetPos, Action onDone, bool instant, bool playSound)
		{
			BattleCamera._onDone = onDone;
			MoveToPoint(targetPos, instant);
		}

		public static void Move(Vector3 targetPos, Action onDone, bool instant, params string[] ignoredModules)
		{
			Move(targetPos, onDone, instant, false);
		}

		private static void MoveDone()
		{
			if (BattleCamera._onDone != null && !ignoreOnDone)
			{
				BattleCamera._onDone();
				BattleCamera._onDone = null;
			}
			else if (ignoreOnDone)
			{
				ignoreOnDone = false;
			}
		}

		public static void AddCallMovementBack(Action act)
		{
			_onDone += act;
		}

		public static void MoveToObject(Transform targetPos, Vector3 targetOffset, Action onDone, bool instant, bool playSound)
		{
			BattleCamera._onDone = onDone;
			_instance.fixedOffset = targetOffset;
			_instance._targetTransform = targetPos;
			MoveToPoint(targetPos.position + _instance.fixedOffset, instant);
			if (!instant)
			{
				_instance._cameraState = ECameraState.MOVE_TO_OBJECT;
			}
		}

		public static void MoveToObject(Transform targetPos, Vector3 targetOffset, Action onDone, bool instant)
		{
			MoveToObject(targetPos, targetOffset, onDone, instant, false);
		}

		public static void MoveToObject(Transform targetPos, bool instant)
		{
			MoveToObject(targetPos, Vector3.zero, null, instant, false);
		}

		public static void MoveToObject(Transform targetPos, Vector3 targetOffset, bool instant)
		{
			MoveToObject(targetPos, targetOffset, null, instant, false);
		}

		public static void MoveToStartBattlePosition(bool instant, Action callBack = null)
		{
			if (_instance._cameraState == ECameraState.BATTLE && _instance._startPosition.HasValue)
			{
				MoveToPoint(_instance._startPosition.Value, instant, callBack);
			}
		}

		public static void MoveToDefault(bool instant)
		{
			MoveToPoint(_instance._defaultPosition, instant);
		}

		public static void MoveToPoint(Vector3 targetPos, bool instant, Action callBack = null)
		{
			MoveToPoint(targetPos, Quaternion.identity, instant, callBack);
		}

		public static void MoveToPoint(Vector3 targetPos, Quaternion endRot, bool instant, Action callBack = null, bool ignoreCallback = false)
		{
			ignoreOnDone = ignoreCallback;
			_onDone += callBack;
			_instance.endPos = targetPos;
			_instance.endRotation = endRot;
			_instance.fromClipPlane = _instance.MainCamera.nearClipPlane;
			_instance.toClipPlane = 10f;
			if (instant)
			{
				if (!ignoreCallback)
				{
					_instance._cameraState = ECameraState.STAY;
					_instance.startPos = targetPos;
					_instance.startRot = endRot;
				}
				_instance._transform.position = targetPos;
				_instance._transform.rotation = endRot;
				if (_instance.changeClipPlane)
				{
					_instance.changeClipPlane = false;
				}
				MoveDone();
			}
			else
			{
				_instance._cameraState = ECameraState.MOVE_TO_POINT;
				_instance.startPos = _instance._transform.position;
				_instance.startRot = _instance._transform.rotation;
				_instance.moveTimer = 0f;
				if (_instance.pauseOnMove)
				{
					_instance.startMoveTime = Time.realtimeSinceStartup;
				}
			}
		}

		private void P()
		{
			calculatedPosition.y = _startCameraPosition.y;
		}

		private Vector3 CalculateMaxZ(float playerX, float enemyX)
		{
			float x = enemyX - (enemyX - playerX) / 2f;
			float y = _startCameraPosition.y;
			float z = charsZ - currentMaxDistance * screenShootDistance / _minDist * defOffsetFromChar;
			return new Vector3(x, y, z);
		}

		public static void MoveToSpawnCentre(bool ignoreCallback)
		{
			Vector3 vector = Instance.CalculateMaxZ(SceneConfig.SpawnPointPlayer.x, SceneConfig.SpawnPointEnemy.x);
			_instance.lookAtPointOffset.z = _instance.charsZ - vector.z;
			Quaternion endRot = Quaternion.LookRotation(vector + _instance.lookAtPointOffset - vector);
			MoveToPoint(vector, endRot, true, null, ignoreCallback);
		}

		public static void MoveToDojo(Action onClosed, bool instant)
		{
			BattleCamera._onDone = onClosed;
			_instance.changeClipPlane = true;
			_instance.fromClipPlane = _instance.MainCamera.nearClipPlane;
			_instance.toClipPlane = 10f;
			if (ModelsManager.Instance.Player == null)
			{
				MoveToDefault(false);
			}
			else
			{
				_instance.onlyPlayer = false;
				if (!instant)
				{
					_onDone += _instance.MovedToPlayer;
					_onDone += _instance.ActivateBattleCamera;
				}
				else
				{
					_instance.MainCamera.nearClipPlane = 10f;
				}
				Vector3 vector = _instance.CalculateCameraPos();
				if (_instance.lookAtPlayerPos)
				{
					_instance.lookAtPointOffset.z = _instance.charsZ - vector.z;
				}
				Quaternion endRot = Quaternion.LookRotation(vector + _instance.lookAtPointOffset - vector);
				MoveToPoint(vector, endRot, instant);
				if (instant)
				{
					_instance._cameraState = ECameraState.BATTLE;
				}
			}
			if (_instance.switchCameraTargetSoundName.Length > 0 && !instant)
			{
				AudioManager.Instance.PlaySound(_instance.switchCameraTargetSoundName);
			}
		}

		private void MovedToPlayer()
		{
			if (_instance.pauseOnMove)
			{
				BattleController.ResumeGame();
			}
			_cameraState = ECameraState.BATTLE;
		}

		public static void MoveToPlayer(Vector3 targetOffset, Action onDone, bool instant)
		{
			BattleCamera._onDone = onDone;
			_instance.fixedOffset = targetOffset;
			_instance._targetTransform = ModelsManager.Instance.Player.GetCenterOfMassBone().transform;
			MoveToPoint(_instance._targetTransform.position + _instance.fixedOffset, instant);
			if (!instant)
			{
				_instance._cameraState = ECameraState.MOVE_TO_OBJECT;
			}
		}

		private Vector3 CalculateCameraPos(float playerX, float enemyX)
		{
			calculatedPosition.y = _startCameraPosition.y;
			calculatedPosition.z = _startCameraPosition.z;
			if (onlyPlayer)
			{
				calculatedPosition.x = playerX;
			}
			else
			{
				calculatedPosition.x = enemyX - (enemyX - playerX) / 2f;
				dist = Mathf.Clamp(Math.Abs(enemyX - playerX), _minDist, currentMaxDistance);
				zOffset = dist / _minDist * defOffsetFromChar;
				calculatedPosition.z = charsZ - zOffset;
			}
			return calculatedPosition;
		}

		private Vector3 CalculateCameraPos()
		{
			return CalculateCameraPos(player.modelCollisions.modelBackPos, enemy.modelCollisions.modelBackPos);
		}

		private CameraSettings GetSettingsByAspect(AspectRatio aspect)
		{
			CameraSettings[] array = settings;
			foreach (CameraSettings cameraSettings in array)
			{
				if (cameraSettings.aspect == aspect)
				{
					return cameraSettings;
				}
			}
			return null;
		}

		public void RoundEndTweenMotion()
		{
			CameraMotion = true;
			ModelsManager.Instance.HideModels(0.5f);
			MoveToStartBattlePosition(false, delegate
			{
				CameraMotion = false;
			});
		}

		public bool RoundEndTweenIsReady()
		{
			return !CameraMotion && ModelsManager.Instance.Player.Transparency < 0.001f;
		}

		public void SetCameraBlocked(bool enable)
		{
			BlockCameraMovement = enable;
		}
	}
}
