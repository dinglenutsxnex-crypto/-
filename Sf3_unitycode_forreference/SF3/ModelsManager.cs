using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DynamicShadowProjector;
using SF3.Effects;
using SF3.GameModels;
using SF3.Moves;
using UnityEngine;

namespace SF3
{
	[Serializable]
	public class ModelsManager : MonoBehaviour, ISceneInitializationObject
	{
		private class ModelCreationInfo
		{
			public readonly Model model;

			public readonly int modelCreatorID;

			public ModelCreationInfo(Model modelValue, int modelCreatorIDValue)
			{
				model = modelValue;
				modelCreatorID = modelCreatorIDValue;
			}
		}

		private List<Model> _playerModels;

		private List<Model> _enemyModels;

		private readonly List<Collider> _colliders = new List<Collider>();

		private ModelsAttributesController _modelsAttributesController;

		public List<Renderer> BlurBounds;

		private readonly ModelCollision.CheckCollisionResult collisionResult = new ModelCollision.CheckCollisionResult();

		public static ModelsManager Instance { get; private set; }

		public List<Model> Models { get; private set; }

		public Model Player { get; private set; }

		public Model Enemy { get; private set; }

		public Model CurrentUpdatedModel { get; private set; }

		public event Action OnBattleModelsCreatedEvent;

		private void Awake()
		{
			Instance = this;
			_playerModels = new List<Model>();
			_enemyModels = new List<Model>();
			Models = new List<Model>();
			_modelsAttributesController = new ModelsAttributesController();
		}

		public void Initialize()
		{
			if (this.OnBattleModelsCreatedEvent != null)
			{
				Delegate[] invocationList = this.OnBattleModelsCreatedEvent.GetInvocationList();
				foreach (Delegate @delegate in invocationList)
				{
					OnBattleModelsCreatedEvent -= (Action)@delegate;
				}
			}
		}

		public List<Rule> GetAllRules()
		{
			List<Rule> list = new List<Rule>();
			foreach (Model model in Models)
			{
				list.AddRange(model.modelInfo.rules);
			}
			return list;
		}

		public void DisposePreviousLocation()
		{
			DisposeModel(Enemy);
			DisposeModel(Player);
			DisposeUnusedModels();
			_playerModels.Clear();
			_enemyModels.Clear();
			Models.Clear();
			Enemy = null;
		}

		public void DisableModel(int id)
		{
			DisableModel(id, Models, _playerModels, _enemyModels);
		}

		private void DisableModel(int id, params List<Model>[] modelLists)
		{
			foreach (List<Model> list in modelLists)
			{
				Model model = list.FirstOrDefault((Model m) => m.id == id);
				if (null != model)
				{
					list.Remove(model);
				}
			}
		}

		private void ClearNullModels()
		{
			for (int i = 0; i < Models.Count; i++)
			{
				if (Models[i] == null)
				{
					Models.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < _playerModels.Count; j++)
			{
				if (_playerModels[j] == null)
				{
					_playerModels.RemoveAt(j);
					j--;
				}
			}
			for (int k = 0; k < _enemyModels.Count; k++)
			{
				if (_enemyModels[k] == null)
				{
					_enemyModels.RemoveAt(k);
					k--;
				}
			}
		}

		public static void DestroyModel(int id)
		{
			int num = 0;
			for (num = 0; num < Instance.Models.Count; num++)
			{
				if (Instance.Models[num].id == id)
				{
					GlobalLoad.Unload(Instance.Models[num].gameObject);
					Instance.Models.RemoveAt(num);
					break;
				}
			}
			Instance.ClearNullModels();
		}

		public void DisposeModelsChild(int idParent)
		{
			DisposeModelChildren(idParent);
			ClearNullModels();
		}

		public static void DestroyModel(Model modelState)
		{
			DestroyModel(modelState.id);
		}

		public void SetModelsRagdollSleepState(bool isSleep, int priority)
		{
			foreach (Model model in Models)
			{
				model.SetRagdollSleepState(isSleep, priority);
			}
		}

		public void ClearBattleModels()
		{
			MovesController.ClearActionsCallbacks();
			if (Player != null)
			{
				DisposeModel(Player);
			}
			if (Enemy != null)
			{
				DisposeModel(Enemy);
			}
			DisposeUnusedModels();
			_playerModels.Clear();
			_enemyModels.Clear();
			Models.Clear();
		}

		public void CreateBattleModels(ModelInfo playerInfo, ModelInfo enemyInfo)
		{
			Player = CreateModel(playerInfo, -1, 1);
			Enemy = CreateModel(enemyInfo, -1, 2);
			if (this.OnBattleModelsCreatedEvent != null)
			{
				this.OnBattleModelsCreatedEvent();
			}
			Models = new List<Model> { Player, Enemy };
			using (new TimerNode("Model.Initialize", "ModelsLoad"))
			{
				foreach (Model model in Models)
				{
					model.Initialize(null, false);
				}
			}
			foreach (Model model2 in Models)
			{
				model2.ThrowBirth();
			}
			CollectModelsColliders();
			PerkCarousel.ModelsInitialized();
			MovesController.UnloadUnusedTriggers();
		}

		private void CollectModelsColliders()
		{
			List<Collider> list = new List<Collider>();
			if (Player != null)
			{
				list.AddRange(Player.GetComponentsInChildren<Collider>());
			}
			if (Enemy != null)
			{
				list.AddRange(Enemy.GetComponentsInChildren<Collider>());
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].enabled)
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}

		public void EnableModelsColliders(bool enable)
		{
			_colliders.ForEach(delegate(Collider c)
			{
				c.enabled = enable;
			});
		}

		public Model CreateModelInPull(ModelInfo modelInfo, int modelCreatorID, int modelID = -1)
		{
			Model model = Model.Create(modelInfo, modelID);
			model.transform.parent = base.transform;
			return model;
		}

		public Model CreateModel(ModelInfo modelInfo, int modelCreatorID, int modelID = -1)
		{
			Debug.Log(string.Format("CreateModel [{0}]", modelInfo.alias));
			Model model = Models.FirstOrDefault((Model a) => a.id == modelCreatorID);
			if (model != null)
			{
				modelInfo.SetIsPlayer(model.isPlayer);
			}
			Model model2 = CreateModelInPull(modelInfo, modelCreatorID, modelID);
			if (modelInfo.isPlayer)
			{
				_playerModels.Add(model2);
				if (SceneConfig.IsPresent)
				{
					model2.transform.position = SceneConfig.SpawnPointPlayer;
				}
				else
				{
					Debug.LogError("Can't move player to spawn  - no SceneConfig._instance");
				}
			}
			else
			{
				_enemyModels.Add(model2);
				if (SceneConfig.IsPresent)
				{
					model2.transform.position = SceneConfig.SpawnPointEnemy;
				}
				else
				{
					Debug.LogError("Can't move enemy to spawn  - no SceneConfig._instance");
				}
			}
			Models.Add(model2);
			ShadowSetup(model2);
			return model2;
		}

		private static void ShadowSetup(Model model)
		{
			if ((bool)model)
			{
				Routiner.Go(WaitForShadow(model));
			}
		}

		private static IEnumerator WaitForShadow(Model model)
		{
			while ((bool)model.transform)
			{
				yield return new WaitForEndOfFrame();
				if ((bool)model)
				{
					ShadowTextureRenderer componentInChildren = model.GetComponentInChildren<ShadowTextureRenderer>();
					if ((bool)componentInChildren && (bool)LocationColorAnimation.Instance)
					{
						LocationColorAnimation.Instance.SetShadow(componentInChildren);
						break;
					}
					continue;
				}
				break;
			}
		}

		public void SetModelsActive(bool value)
		{
			foreach (Model model in Models)
			{
				model.Activate(value);
			}
		}

		public void UpdateModels()
		{
			_modelsAttributesController.Update(GameTimeController.gameTimeDelta);
			for (int i = 0; i < Models.Count; i++)
			{
				if (Models[i].gameObject.activeSelf)
				{
					CurrentUpdatedModel = Models[i];
					TriggersVerification.currentVerificationData.SetModel(Models[i]);
					Models[i].UpdateAnimation();
					Models[i].UpdateEquationLineCollision();
					Models[i].UpdateShadow();
					Models[i].modelComponents.UpdateCenterOffMass();
					Models[i].UpdateFloor();
					Models[i].statusControl.UpdateStatuses();
					Models[i].UpdateAi();
				}
			}
			for (int j = 0; j < Models.Count; j++)
			{
				CurrentUpdatedModel = Models[j];
				Models[j].modelCollisions.CheckBorderHit();
			}
			UpdateCollisions();
			for (int k = 0; k < Models.Count; k++)
			{
				Models[k].modelComponents.UpdateCenterOffMass(true);
			}
		}

		public void UpdateCollisions()
		{
			bool flag = false;
			if (CheckModelsAttackCollisions())
			{
				return;
			}
			foreach (Model model in Models)
			{
				if (model.GetIntervalExist(EIntervalsType.INTERVAL_NO_REPULSION).Count <= 0)
				{
					flag = model.modelCollisions.CheckRepulsion();
					if (flag)
					{
						break;
					}
				}
			}
			foreach (Model model2 in Models)
			{
				if (model2.GetIntervalExist(EIntervalsType.INTERVAL_NO_WALL_REPULSION).Count > 0 || !model2.modelCollisions.CheckWalls(flag))
				{
					continue;
				}
				break;
			}
		}

		private bool CheckModelsAttackCollisions()
		{
			if (_enemyModels.Count == 0 || !_enemyModels[0].active)
			{
				return false;
			}
			collisionResult.isCollision = false;
			collisionResult.isMainCollision = false;
			if (GameTimeController.frameCount % 2 == 0)
			{
				CheckCollisionsForModels(_playerModels);
				if (!collisionResult.isMainCollision)
				{
					CheckCollisionsForModels(_enemyModels);
				}
			}
			else
			{
				CheckCollisionsForModels(_enemyModels);
				if (!collisionResult.isMainCollision)
				{
					CheckCollisionsForModels(_playerModels);
				}
			}
			return collisionResult.isCollision;
		}

		private void CheckCollisionsForModels(List<Model> models)
		{
			foreach (Model model in models)
			{
				if (model.modelCollisions.CheckAttackCollisions())
				{
					collisionResult.isCollision = true;
					if (model.parentModel == null)
					{
						collisionResult.isMainCollision = true;
					}
				}
			}
		}

		public EDirectionType GetFoeDirection(Model modelState)
		{
			EDirectionType result = EDirectionType.NONE;
			if (modelState.isPlayer)
			{
				if (Enemy != null)
				{
					result = ((Enemy.modelComponents.rootBone.position.x > modelState.modelComponents.rootBone.position.x) ? EDirectionType.RIGHT : EDirectionType.LEFT);
				}
			}
			else if (Player != null)
			{
				result = ((Player.modelComponents.rootBone.position.x > modelState.modelComponents.rootBone.position.x) ? EDirectionType.RIGHT : EDirectionType.LEFT);
			}
			return result;
		}

		public void DisposeUnusedModels()
		{
		}

		public void DisposeModel(Model mainModelToDispose)
		{
			if (!mainModelToDispose)
			{
				return;
			}
			foreach (Model childModel in mainModelToDispose.childModels)
			{
				GlobalLoad.Unload(childModel.gameObject);
			}
			GlobalLoad.Unload(mainModelToDispose.gameObject);
			EffectsManager.Instance.DisposeEffectsByModel(mainModelToDispose.id);
		}

		public void DisposeModelChildren(int modelParentID)
		{
			for (int i = 0; i < Models.Count; i++)
			{
				if (Models[i].parentModel != null && Models[i].parentModel.id == modelParentID)
				{
					GlobalLoad.Unload(Models[i].gameObject);
					EffectsManager.Instance.DisposeEffectsByModel(Models[i].id);
					Models.RemoveAt(i);
					i--;
				}
			}
		}

		private void ShowModels(bool show, float animationDuration, params Model[] allmodels)
		{
			foreach (Model model in allmodels)
			{
				if (null != model && ((show && model.Transparency < 0.999f) || (!show && model.Transparency > 0.001f) || model.IsAnimateTransparentInprogress()))
				{
					model.Fade(animationDuration, show);
				}
			}
		}

		public Model GetModelById(int id)
		{
			return Models.First((Model m) => m.id == id);
		}

		public void ShowPlayer(float fadeModelDuration = 0f)
		{
			ShowModels(true, fadeModelDuration, Player);
		}

		public void HidePlayer(float fadeModelDuration = 0f)
		{
			ShowModels(false, fadeModelDuration, Player);
		}

		public void ShowEnemy(float fadeModelDuration = 0f)
		{
			ShowModels(true, fadeModelDuration, Enemy);
		}

		public void HideEnemy(float fadeModelDuration = 0f)
		{
			ShowModels(false, fadeModelDuration, Enemy);
		}

		public void ShowModels(float animationDuration = 0f)
		{
			ShowModels(true, animationDuration, Player, Enemy);
		}

		public void HideModels(float animationDuration = 0f)
		{
			ShowModels(false, animationDuration, Player, Enemy);
		}
	}
}
