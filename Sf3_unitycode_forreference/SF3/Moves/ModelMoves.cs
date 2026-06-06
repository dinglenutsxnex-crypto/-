using System;
using System.Collections.Generic;
using System.Linq;
using Nekki;
using SF3.GameModels;
using SF3.Items;

namespace SF3.Moves
{
	public class ModelMoves
	{
		protected class TriggersByItem
		{
			public List<InfoTrigger> moveTriggers;

			public List<InfoTrigger>[] moveTriggersByEvents;

			public TriggersByItem(Model modelState, List<InfoTriggerSet> moveTriggersAll, ref Dictionary<string, ModelInfoAnimation> animNamesCompliance)
			{
				CollectTriggersFromSets(moveTriggersAll);
				CreateTriggersByEvents();
				InitTriggersActionsFunc(modelState, ref animNamesCompliance);
			}

			private void CollectTriggersFromSets(List<InfoTriggerSet> moveTriggersAll)
			{
				moveTriggers = new List<InfoTrigger>();
				foreach (InfoTriggerSet item in moveTriggersAll)
				{
					foreach (InfoTrigger trigger in item.triggers)
					{
						moveTriggers.Add(trigger);
					}
				}
			}

			private void CreateTriggersByEvents()
			{
				int count = EnumsCompliancer.GetEnumerators<ETriggerEvents>().Count;
				moveTriggersByEvents = new List<InfoTrigger>[count];
				for (int i = 0; i < moveTriggersByEvents.Length; i++)
				{
					moveTriggersByEvents[i] = new List<InfoTrigger>();
				}
				foreach (InfoTrigger moveTrigger in moveTriggers)
				{
					foreach (TriggerEvent @event in moveTrigger.events)
					{
						moveTriggersByEvents[(int)@event.type].Add(moveTrigger);
					}
				}
			}

			private void InitTriggersActionsFunc(Model modelState, ref Dictionary<string, ModelInfoAnimation> animNamesCompliance)
			{
				foreach (InfoTrigger moveTrigger in moveTriggers)
				{
					foreach (ITriggerAction action in moveTrigger.actions)
					{
						if (action.type == TriggerAction.EActionType.ANIMATION || action.type == TriggerAction.EActionType.ANIMATION_RANDOM)
						{
							CreateTriggersAnimations(action, modelState, ref animNamesCompliance);
						}
					}
				}
			}

			private void CreateTriggersAnimations(ITriggerAction actionsVal, Model modelState, ref Dictionary<string, ModelInfoAnimation> animNamesCompliance)
			{
				TriggerActionAnimation triggerActionAnimation = actionsVal as TriggerActionAnimation;
				if (triggerActionAnimation != null)
				{
					AddAnimationName(triggerActionAnimation.name, modelState, ref animNamesCompliance);
					return;
				}
				TriggerActionAnimationRandom triggerActionAnimationRandom = actionsVal as TriggerActionAnimationRandom;
				if (triggerActionAnimationRandom != null)
				{
					string[] arrayNames = triggerActionAnimationRandom.ArrayNames;
					foreach (string name in arrayNames)
					{
						AddAnimationName(name, modelState, ref animNamesCompliance);
					}
				}
			}

			private void AddAnimationName(string name, Model modelState, ref Dictionary<string, ModelInfoAnimation> animNamesCompliance)
			{
				if (!animNamesCompliance.ContainsKey(name))
				{
					InfoAnimation animationByName = MovesController.GetAnimationByName(name);
					if (animationByName == null)
					{
						throw new Exception(string.Format("Animation with name [{0}] does not exist", name));
					}
					ModelInfoAnimation modelInfoAnimation = new ModelInfoAnimation(animationByName, modelState);
					animNamesCompliance.Add(modelInfoAnimation.animation.name, modelInfoAnimation);
				}
			}

			public void RestrictAnimations(List<string> animations)
			{
				foreach (string animation in animations)
				{
					for (int i = 0; i < moveTriggers.Count; i++)
					{
						if (!moveTriggers[i].animationName.Equals(animation))
						{
							continue;
						}
						foreach (TriggerEvent @event in moveTriggers[i].events)
						{
							int type = (int)@event.type;
							for (int j = 0; j < moveTriggersByEvents[type].Count; j++)
							{
								if (moveTriggersByEvents[type][j] == moveTriggers[i])
								{
									moveTriggersByEvents[type].RemoveAt(j);
									j--;
									break;
								}
							}
						}
						moveTriggers.RemoveAt(i);
						i--;
					}
				}
			}
		}

		protected Dictionary<string, ModelInfoAnimation> _animNamesCompliance;

		protected TriggersByItem _currentTriggers;

		protected TriggersByItem _disarmTriggers;

		protected TriggersByItem _normalTriggers;

		public List<InfoTrigger> currentTriggers
		{
			get
			{
				return _currentTriggers.moveTriggers;
			}
		}

		public ModelMoves()
		{
			_animNamesCompliance = new Dictionary<string, ModelInfoAnimation>();
		}

		public void LoadMoves(Model modelState)
		{
			_animNamesCompliance.Clear();
			List<InfoTriggerSet> list = MovesController.LoadTriggers(modelState);
			_normalTriggers = new TriggersByItem(modelState, list, ref _animNamesCompliance);
			Equipment equipped = modelState.modelInfo.GetEquipped(EquipmentType.Weapon);
			list.Clear();
			Equipment defaultEquipment = modelState.modelInfo.GetDefaultEquipment(EquipmentType.Weapon);
			modelState.modelInfo.EquipItemNotExisted(defaultEquipment);
			list = MovesController.LoadTriggers(modelState);
			_disarmTriggers = new TriggersByItem(modelState, list, ref _animNamesCompliance);
			if (equipped != null)
			{
				if (equipped.id != defaultEquipment.id)
				{
					modelState.modelInfo.EquipItemNotExisted(equipped);
					defaultEquipment.SetEquipped(false);
				}
			}
			else
			{
				modelState.modelInfo.EquipItemNotExisted(defaultEquipment);
				defaultEquipment.SetEquipped(true);
			}
			PerkCarousel.Init(_disarmTriggers.moveTriggers);
			PerkCarousel.Init(_normalTriggers.moveTriggers);
			_currentTriggers = _normalTriggers;
		}

		public void SwitchToDisarm(bool isPlayer)
		{
			_currentTriggers = _disarmTriggers;
			if (isPlayer)
			{
				StickHelper.Instance.SetDisarm();
			}
		}

		public List<InfoTrigger> GetTriggersByEvent(ETriggerEvents eventType)
		{
			return _currentTriggers.moveTriggersByEvents[(int)eventType];
		}

		public ModelInfoAnimation GetModelAnimationByName(string name)
		{
			if (_animNamesCompliance.ContainsKey(name))
			{
				return _animNamesCompliance[name];
			}
			return null;
		}

		public void RestrictAnimations(List<string> animations)
		{
			List<string> list = new List<string>();
			foreach (string animation in animations)
			{
				for (int i = 0; i < _animNamesCompliance.Count; i++)
				{
					KeyValuePair<string, ModelInfoAnimation> keyValuePair = _animNamesCompliance.ElementAt(i);
					if (keyValuePair.Value.animation.HasNameOrGroup(animation))
					{
						list.Add(keyValuePair.Key);
						_animNamesCompliance.Remove(keyValuePair.Key);
						i--;
					}
				}
			}
			_normalTriggers.RestrictAnimations(list);
			_disarmTriggers.RestrictAnimations(list);
		}
	}
}
