using System;
using System.Collections.Generic;
using System.IO;
using SF3.GameModels;
using SF3.Moves;
using SF3.Settings;
using UnityEngine;

namespace SF3.Tactics
{
	public class ReactionAnalyzer : IReactionAnalyzer
	{
		public const string COMPLIANCE_TABLE_NAME = "COMPLIANCE.bytes";

		public const string REVERSE_RESULT_PREFIX = "REVERSE_";

		private readonly Model _model;

		private readonly Model _enemy;

		private readonly List<InfoTrigger> _emptyList;

		private const string _tablePath = "results_tables";

		private Dictionary<int, string> compliance;

		public Dictionary<string, List<InfoTrigger>> animationTriggers;

		public Dictionary<Model, InfoBones> tactictsBones;

		public ReactionAnalyzer(Model model)
		{
			_model = model;
			_enemy = model.enemy;
			_emptyList = new List<InfoTrigger>();
			animationTriggers = new Dictionary<string, List<InfoTrigger>>();
			InitComplaince();
			InitBones();
		}

		private void InitComplaince()
		{
			byte[] loadBytesInternal = GlobalLoad.GetLoadBytesInternal("results_tables", "COMPLIANCE.bytes");
			if (loadBytesInternal.Length > 0)
			{
				BinaryReader binaryReader = new BinaryReader(new MemoryStream(loadBytesInternal));
				compliance = new Dictionary<int, string>();
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string value = binaryReader.ReadString();
					int key = binaryReader.ReadInt32();
					compliance[key] = value;
				}
			}
		}

		private void InitTriggers()
		{
			animationTriggers.Clear();
			foreach (string value in compliance.Values)
			{
				foreach (InfoTrigger currentTrigger in _model.modelMoves.currentTriggers)
				{
					if (currentTrigger.animationName.Equals(value))
					{
						if (!animationTriggers.ContainsKey(value))
						{
							animationTriggers[value] = new List<InfoTrigger>();
						}
						animationTriggers[value].Add(currentTrigger);
					}
				}
			}
		}

		private void InitBones()
		{
			tactictsBones = new Dictionary<Model, InfoBones>();
			string pivotNameByParam = GetPivotNameByParam(TacticsSettings.TacticsPivot);
			string pivotNameByParam2 = GetPivotNameByParam(TacticsSettings.TacticsMirroredPivot);
			InitModelPivotBone(_model, pivotNameByParam, pivotNameByParam2);
			InitModelPivotBone(_enemy, pivotNameByParam, pivotNameByParam2);
		}

		private string GetPivotNameByParam(string paramName)
		{
			return TacticsSettings.GetParamByName(paramName);
		}

		private void InitModelPivotBone(Model currentModel, string pivotName, string mirrorName)
		{
			InfoBones infoBones = new InfoBones();
			infoBones.pivotBone = GetModelPivotBone(currentModel, pivotName);
			infoBones.mirrorBone = GetModelPivotBone(currentModel, mirrorName);
			InfoBones value = infoBones;
			tactictsBones[currentModel] = value;
		}

		private Bone GetModelPivotBone(Model currentModel, string name)
		{
			return (name == null) ? currentModel.GetPivotBone() : currentModel.GetBone(name);
		}

		private Bone GetBone(Model currentModel, bool reverse = false)
		{
			InfoBones infoBones = tactictsBones[currentModel];
			if (currentModel.moveControl.mirrored)
			{
				return reverse ? infoBones.pivotBone : infoBones.mirrorBone;
			}
			return reverse ? infoBones.mirrorBone : infoBones.pivotBone;
		}

		private float GetDistance()
		{
			int enemySign = _model.GetEnemySign();
			float x = GetBone(_model, enemySign == -1).position.x;
			float x2 = GetBone(_enemy).position.x;
			return Math.Abs(x - x2) * (float)_enemy.GetEnemySign();
		}

		private Product GetProduct()
		{
			InitTriggers();
			InfoAnimation animation = _enemy.modelAnimation.mainAnimationInfo.animation;
			float delta = _enemy.modelAnimation.delta + 1f / (float)animation.midFrames;
			bool reverseResult = _enemy.GetEnemySign() == -1;
			return ReadBinary(animation.name, delta, reverseResult);
		}

		private List<InfoTrigger> GetTriggers(List<InfoProduct> products)
		{
			List<InfoTrigger> list = new List<InfoTrigger>();
			foreach (InfoProduct product in products)
			{
				List<InfoTrigger> triggersByProduct = GetTriggersByProduct(product);
				if (triggersByProduct != null)
				{
					list.AddRange(triggersByProduct);
				}
			}
			return list;
		}

		private List<InfoTrigger> GetTriggersByProduct(InfoProduct product)
		{
			return (!animationTriggers.ContainsKey(product.animationName)) ? null : animationTriggers[product.animationName];
		}

		private Product ReadBinary(string animation, float delta, bool reverseResult)
		{
			Product product = null;
			byte[] array = ((!reverseResult) ? GlobalLoad.GetLoadBytesInternal("results_tables", animation) : GlobalLoad.GetLoadBytesInternal("results_tables", "REVERSE_" + animation));
			if (array.Length > 0)
			{
				product = new Product();
				BinaryReader binaryReader = new BinaryReader(new MemoryStream(array));
				float distance = GetDistance();
				int num = binaryReader.ReadInt32();
				int num2 = binaryReader.ReadInt32();
				int num3 = Mathf.RoundToInt(delta * (float)num);
				for (int i = 0; i < num2; i++)
				{
					int num4 = binaryReader.ReadInt32();
					int num5 = binaryReader.ReadInt32();
					string animationName = compliance[num4];
					List<InfoProduct> list = new List<InfoProduct>();
					for (int j = 0; j < num5; j++)
					{
						bool flag = j == num3;
						int num6 = binaryReader.ReadInt32();
						for (int k = 0; k < num6; k++)
						{
							InfoType infoType = (InfoType)binaryReader.ReadByte();
							float num7 = binaryReader.ReadSingle();
							float num8 = binaryReader.ReadSingle();
							float baseDamage = binaryReader.ReadSingle();
							int frameIndex = binaryReader.ReadInt32();
							if (flag && distance >= num7 && distance <= num8)
							{
								InfoProduct infoProduct = new InfoProduct();
								infoProduct.baseDamage = baseDamage;
								infoProduct.animationIndex = num4;
								infoProduct.animationName = animationName;
								infoProduct.frameIndex = frameIndex;
								infoProduct.infoType = infoType;
								InfoProduct item = infoProduct;
								list.Add(item);
							}
						}
					}
					InfoProduct infoProduct2 = GetInfoProduct(list);
					if (infoProduct2 == null)
					{
						InfoProduct infoProduct = new InfoProduct();
						infoProduct.animationName = animationName;
						infoProduct.animationIndex = num4;
						infoProduct.infoType = InfoType.NONE;
						infoProduct2 = infoProduct;
					}
					product.Add(infoProduct2);
				}
				binaryReader.Close();
			}
			return product;
		}

		private InfoProduct GetInfoProduct(List<InfoProduct> products)
		{
			InfoProduct result = null;
			foreach (InfoProduct product in products)
			{
				switch (product.infoType)
				{
				case InfoType.BOTH:
				case InfoType.HIT_B:
					return product;
				case InfoType.HIT_A:
					result = product;
					break;
				}
			}
			return result;
		}

		[Obsolete("Deprecated, please use GetReactionAttack() and/or GetReactionDodge() instead.")]
		public List<InfoTrigger> GetReaction()
		{
			Product product = GetProduct();
			if (product != null)
			{
				if (product.attackList.Count > 0)
				{
					return GetTriggers(product.attackList);
				}
				if (product.dodgeList.Count > 0)
				{
					return GetTriggers(product.dodgeList);
				}
			}
			return null;
		}

		public List<InfoTrigger> GetReactionAttack()
		{
			Product product = GetProduct();
			return (product == null) ? _emptyList : GetTriggers(product.attackList);
		}

		public List<InfoTrigger> GetReactionDodge()
		{
			Product product = GetProduct();
			return (product == null) ? _emptyList : GetTriggers(product.dodgeList);
		}
	}
}
