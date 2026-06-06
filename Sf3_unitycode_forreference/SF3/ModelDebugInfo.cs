using System.Collections.Generic;
using Nekki;
using SF3.GameModels;
using SF3.KeyPressInfo;
using SF3.Moves;
using UnityEngine;

namespace SF3
{
	public class ModelDebugInfo : UIModuleHolder
	{
		private enum Tabs
		{
			Main = 0,
			Tags = 1
		}

		[SerializeField]
		private UILabel _dirrections;

		[SerializeField]
		private MultiTweenTransition _multiTween;

		[SerializeField]
		private UIButton _bt;

		[SerializeField]
		private UIButton _btMain;

		[SerializeField]
		private UIButton _btTags;

		private bool _active = true;

		private Model _modelState;

		private string animationName;

		private string blendedAnimations;

		private string intervalsName;

		private string animationFrame;

		private string mirrored;

		private string backwarded;

		private string direction;

		private string keyInverted;

		private string pivotBone;

		private string distanceToEnemy;

		private string id;

		private string aiActive;

		private string currentnKeys;

		private string rootBone;

		public GameObject objToDisable;

		private string factors;

		private string variables;

		private string repRect;

		private Tabs _currentTab;

		internal void Start()
		{
			if (NekkiUtils.IsDebug)
			{
				if (_modelState.isPlayer)
				{
					DebugInfoHider.setPlayerStatsObj = base.gameObject;
				}
				else
				{
					DebugInfoHider.setEnemyStatsObj = base.gameObject;
				}
				_bt.onClick.Add(new EventDelegate(ShowHide));
				ShowHide();
			}
			_btMain.onClick.Add(new EventDelegate(delegate
			{
				_currentTab = Tabs.Main;
			}));
			_btTags.onClick.Add(new EventDelegate(delegate
			{
				_currentTab = Tabs.Tags;
			}));
		}

		private void Update()
		{
			if (!_active)
			{
				return;
			}
			if (_currentTab == Tabs.Tags)
			{
				if ((bool)_modelState && _modelState.modelInfo != null)
				{
					_dirrections.text = string.Join("\n", _modelState.modelInfo.GetEquippedTags().ToArray());
				}
				return;
			}
			if (_modelState.animationInfo != null)
			{
				animationName = _modelState.animationInfo.animation.name;
				blendedAnimations = string.Empty;
				string[] playingAnimations = _modelState.modelAnimation.GetPlayingAnimations();
				foreach (string text in playingAnimations)
				{
					blendedAnimations = blendedAnimations + "\n     " + text;
				}
				if (_modelState.animationIntervals != null)
				{
					intervalsName = string.Empty;
					foreach (IntervalAnimation animationInterval in _modelState.animationIntervals)
					{
						intervalsName = intervalsName + "\n     " + animationInterval.name;
					}
				}
				animationFrame = _modelState.modelAnimation.animationKey + 1 + string.Empty;
				backwarded = _modelState.moveControl.moveDirection.ToString();
				direction = _modelState.moveControl.directionSign + string.Empty;
				mirrored = _modelState.moveControl.mirrored + string.Empty;
				keyInverted = _modelState.keyManager.IsInverted() + string.Empty;
				pivotBone = _modelState.modelComponents.pivotBone.boneName;
				if (ModelsManager.Instance.Enemy.Ai == null)
				{
					distanceToEnemy = "enemy duno about this(";
				}
				id = _modelState.id + string.Empty;
				aiActive = _modelState.IsAI + string.Empty;
				currentnKeys = string.Empty;
				List<KeyData> keys = _modelState.keyManager.GetKeys();
				if (keys != null)
				{
					foreach (KeyData item in keys)
					{
						currentnKeys = currentnKeys + "\n" + item;
					}
				}
				rootBone = _modelState.modelComponents.rootBone.boneName;
				factors = string.Empty;
				List<ModelAttributesModifier.ModifiedAttribute> modelModifiedAttributes = ModelsAttributesController.Instance.GetModelModifiedAttributes(_modelState.id);
				foreach (ModelAttributesModifier.ModifiedAttribute item2 in modelModifiedAttributes)
				{
					string text2 = factors;
					factors = string.Concat(text2, "\n     ", item2.attribute, "  [ ", item2.value, " ]");
				}
				variables = string.Empty;
				foreach (GameVariables.LocalVariable item3 in GameVariables.GetVariablesByOwner(_modelState.id))
				{
					string text2 = variables;
					variables = text2 + "\n     " + item3.name + "  [ " + item3.ToString() + " ]";
				}
				RepulsionRect repulsionRect = _modelState.modelComponents.modelCapsules.repulsionRect;
				repRect = Mathf.RoundToInt(repulsionRect.pointLeft) + " " + Mathf.RoundToInt(repulsionRect.pointRight) + " " + Mathf.RoundToInt(repulsionRect.pointUp) + " " + Mathf.RoundToInt(repulsionRect.pointBot);
			}
			else
			{
				animationName = string.Empty;
				blendedAnimations = string.Empty;
				intervalsName = string.Empty;
				animationFrame = string.Empty;
				backwarded = string.Empty;
				mirrored = string.Empty;
				keyInverted = string.Empty;
				pivotBone = string.Empty;
				distanceToEnemy = string.Empty;
				id = string.Empty;
				aiActive = false + string.Empty;
				currentnKeys = string.Empty;
				rootBone = string.Empty;
				factors = string.Empty;
				variables = string.Empty;
			}
			_dirrections.text = string.Format("MainAnimation: {0} \n BlendedAnimations: {1} \n Interval: {2} \n Animation key: {3} \n Backward: {4} \n Mirror: {5} \n Direction: {6} \n Key inverted: {7} \n Pivot bone: {8} \n Distance to enemy: {9,6:f} \n ID: {10} \n AI active: {11} \n Keys: {12} \n Root bone: {13} \n Factors: {14} \n Variables: {15} \n Repulsion: {16} ", animationName, blendedAnimations, intervalsName, animationFrame, backwarded, mirrored, direction, keyInverted, pivotBone, distanceToEnemy, id, aiActive, currentnKeys, rootBone, factors, variables, repRect);
			_dirrections.text += "\n";
		}

		public void ShowHide()
		{
			_active = !_active;
			objToDisable.SetActive(_active);
		}

		private void OnButtonClick()
		{
			if (_active)
			{
				Hide();
			}
			else
			{
				Show();
			}
		}

		public void Init(Model modelState)
		{
			_modelState = modelState;
		}

		public void SetButtonPositionX(float newPositionX)
		{
			Vector3 localPosition = _bt.transform.localPosition;
			localPosition.x = newPositionX;
			_bt.transform.localPosition = localPosition;
		}

		private void Show()
		{
			_bt.normalSprite = "Debug_minimize";
			_multiTween.TweenIn();
			_active = true;
		}

		private void Hide()
		{
			_bt.normalSprite = "Debug_expand";
			_multiTween.TweenOut();
			_active = false;
		}
	}
}
