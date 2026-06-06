using System.Collections.Generic;
using System.IO;
using System.Linq;
using SF3.Moves;
using UnityEngine;

namespace SF3.Tactics
{
	public class ResultsTableAnalyzer
	{
		public class Product
		{
			public List<int> attackAnimations { get; private set; }

			public List<int> gothitAnimations { get; private set; }

			public List<int> attackFramesOnRanges { get; private set; }

			public int startFrame { get; private set; }

			public float distance { get; private set; }

			public bool idleIsSafe { get; private set; }

			public Product(List<int> attackAnimations, List<int> gothitAnimations, List<int> attackFramesOnRanges, int startFrame, float distance, bool idleIsSafe)
			{
				this.attackAnimations = attackAnimations;
				this.gothitAnimations = gothitAnimations;
				this.attackFramesOnRanges = attackFramesOnRanges;
				this.startFrame = startFrame;
				this.distance = distance;
				this.idleIsSafe = idleIsSafe;
			}
		}

		private const string IDLE_GROUP = "stance_idle";

		private readonly HashSet<int> _animationSet;

		private readonly HashSet<int> _idleSet;

		private Product _product;

		private const string _resultTablePath = "results_tables";

		private readonly Dictionary<string, int> _complianceTable;

		public ResultsTableAnalyzer(List<string> animationNames)
		{
			byte[] loadBytesInternal = GlobalLoad.GetLoadBytesInternal("results_tables", "COMPLIANCE.bytes");
			if (loadBytesInternal.Length > 0)
			{
				MemoryStream input = new MemoryStream(loadBytesInternal);
				BinaryReader binaryReader = new BinaryReader(input);
				_complianceTable = new Dictionary<string, int>();
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					_complianceTable[binaryReader.ReadString()] = binaryReader.ReadInt32();
				}
			}
			_animationSet = new HashSet<int>(animationNames.FindAll((string a) => _complianceTable.ContainsKey(a)).ConvertAll((string a) => _complianceTable[a]));
			_idleSet = new HashSet<int>(animationNames.FindAll((string a) => _complianceTable.ContainsKey(a) && MovesController.GetAnimationByName(a).groupNames.Contains("stance_idle")).ConvertAll((string a) => _complianceTable[a]));
		}

		public void Analize(string animation, float delta, float distance, bool reverseResult)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			int num = 0;
			byte[] array = ((!reverseResult) ? GlobalLoad.GetLoadBytes("results_tables/" + animation) : GlobalLoad.GetLoadBytes("results_tables/REVERSE_" + animation));
			if (array.Length <= 0)
			{
				return;
			}
			MemoryStream input = new MemoryStream(array);
			BinaryReader binaryReader = new BinaryReader(input);
			int num2 = binaryReader.ReadInt32();
			int num3 = binaryReader.ReadInt32();
			num = Mathf.RoundToInt(delta * (float)num2);
			bool flag = true;
			for (int i = 0; i < num3; i++)
			{
				int item = binaryReader.ReadInt32();
				bool flag2 = _animationSet.Contains(item);
				int num4 = binaryReader.ReadInt32();
				for (int j = 0; j < num4; j++)
				{
					bool flag3 = j == num;
					bool flag4 = true;
					int num5 = binaryReader.ReadInt32();
					for (int k = 0; k < num5; k++)
					{
						HitRange.Type type = (HitRange.Type)binaryReader.ReadByte();
						float num6 = binaryReader.ReadSingle();
						float num7 = binaryReader.ReadSingle();
						float num8 = binaryReader.ReadSingle();
						int item2 = binaryReader.ReadInt32();
						if (flag4 && flag3 && flag2 && distance >= num6 && distance <= num7)
						{
							flag4 = false;
							switch (type)
							{
							case HitRange.Type.bHitsA:
								list.Add(item);
								list2.Add(item2);
								break;
							case HitRange.Type.random:
							case HitRange.Type.aHitsB:
								list3.Add(item);
								flag &= !_idleSet.Contains(item);
								break;
							}
						}
					}
				}
			}
			_product = new Product(list, list3, list2, num, distance, flag);
			binaryReader.Close();
		}

		public Product GetProduct()
		{
			if (_product == null)
			{
			}
			return _product;
		}

		public int GetCompliance(string animationName)
		{
			if (_complianceTable.ContainsKey(animationName))
			{
				return _complianceTable[animationName];
			}
			Debug.LogWarning("Animation witn name " + animationName + " not found. MB you have obsolete results tables ");
			return -1;
		}

		public bool ResultTableExists(string animationName)
		{
			return GlobalLoad.GetLoadBytes("results_tables/" + animationName).Length > 0;
		}
	}
}
