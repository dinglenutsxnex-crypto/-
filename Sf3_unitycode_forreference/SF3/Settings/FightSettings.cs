using System.Collections.Generic;
using System.Globalization;
using Nekki.Yaml;
using SF3.Items;
using SF3.Moves;
using UnityEngine;

namespace SF3.Settings
{
	public static class FightSettings
	{
		public class EventSettings
		{
			public ETriggerEvents type;

			public Dictionary<string, string> properties;

			public EventSettings(ETriggerEvents type)
			{
				this.type = type;
				properties = new Dictionary<string, string>();
			}
		}

		public class FightParamsBase
		{
			public FightParamsBase()
			{
			}

			public FightParamsBase(Mapping source)
			{
			}
		}

		public class EventsParams
		{
			public EventsParams()
			{
			}

			public EventsParams(Mapping source)
			{
			}
		}

		public class WallReactionParams : FightParamsBase
		{
			public float coefficient { get; private set; }

			public WallReactionParams(Mapping source)
			{
				Scalar text = source.GetText("Coefficient");
				if (text != null)
				{
					coefficient = float.Parse(text.value.ToString(), CultureInfo.InvariantCulture);
				}
			}
		}

		public class DisarmTrajectory : FightParamsBase
		{
			public Vector3 directionMin;

			public Vector3 directionMax;

			public float impulseMin;

			public float impulseMax;

			public Vector3 torqueMin;

			public Vector3 torqueMax;

			public DisarmTrajectory(Mapping source)
			{
				Sequence sequence = source.GetSequence("DirectionMin");
				directionMin.x = float.Parse(((Scalar)sequence.nodesInside[0]).text, CultureInfo.InvariantCulture);
				directionMin.y = float.Parse(((Scalar)sequence.nodesInside[1]).text, CultureInfo.InvariantCulture);
				directionMin.z = float.Parse(((Scalar)sequence.nodesInside[2]).text, CultureInfo.InvariantCulture);
				sequence = source.GetSequence("DirectionMax");
				directionMax.x = float.Parse(((Scalar)sequence.nodesInside[0]).text, CultureInfo.InvariantCulture);
				directionMax.y = float.Parse(((Scalar)sequence.nodesInside[1]).text, CultureInfo.InvariantCulture);
				directionMax.z = float.Parse(((Scalar)sequence.nodesInside[2]).text, CultureInfo.InvariantCulture);
				Scalar text = source.GetText("ImpulseMin");
				impulseMin = float.Parse(text.text, CultureInfo.InvariantCulture);
				text = source.GetText("ImpulseMax");
				impulseMax = float.Parse(text.text, CultureInfo.InvariantCulture);
				sequence = source.GetSequence("TorqueMin");
				torqueMin.x = float.Parse(((Scalar)sequence.nodesInside[0]).text, CultureInfo.InvariantCulture);
				torqueMin.y = float.Parse(((Scalar)sequence.nodesInside[1]).text, CultureInfo.InvariantCulture);
				torqueMin.z = float.Parse(((Scalar)sequence.nodesInside[2]).text, CultureInfo.InvariantCulture);
				sequence = source.GetSequence("TorqueMax");
				torqueMax.x = float.Parse(((Scalar)sequence.nodesInside[0]).text, CultureInfo.InvariantCulture);
				torqueMax.y = float.Parse(((Scalar)sequence.nodesInside[1]).text, CultureInfo.InvariantCulture);
				torqueMax.z = float.Parse(((Scalar)sequence.nodesInside[2]).text, CultureInfo.InvariantCulture);
			}
		}

		public class ShadowFormParams : FightParamsBase
		{
			public float burnDownPerFrame;

			public ShadowFormParams(Mapping source)
			{
				Scalar text = source.GetText("BurnDownPerFrame");
				if (text != null)
				{
					burnDownPerFrame = float.Parse(text.value.ToString(), CultureInfo.InvariantCulture);
				}
			}
		}

		public class HotGroundParams : FightParamsBase
		{
			public float height;

			public HotGroundParams(Mapping source)
			{
				Scalar text = source.GetText("Height");
				if (text != null)
				{
					height = float.Parse(text.value.ToString(), CultureInfo.InvariantCulture);
				}
			}
		}

		private static EventSettings[] _eventsSettings;

		private static Dictionary<string, FightParamsBase> _fightSettings;

		public static float wallRepulsionCoefficient { get; private set; }

		public static ShadowFormParams shadowFormParams { get; private set; }

		public static Equipment[] defaultEquipments { get; private set; }

		public static string defaultSkeleton { get; private set; }

		public static string[] defaultTags { get; private set; }

		public static string defaultStageIcon { get; private set; }

		public static void Init()
		{
			_fightSettings = new Dictionary<string, FightParamsBase>();
			string fightSettings = ConfigsSourceResolver.FightSettings;
			YamlDocumentNekki yamlDocumentNekki = YamlDocumentNekki.FromYamlContent(fightSettings);
			Mapping mapping = yamlDocumentNekki.GetRoot().GetMapping("Settings");
			Sequence sequence = mapping.GetSequence("Events");
			_eventsSettings = new EventSettings[sequence.nodesInside.Count];
			if (sequence != null)
			{
				for (int i = 0; i < sequence.nodesInside.Count; i++)
				{
					if (!(sequence.nodesInside[i] is Mapping) || !sequence.nodesInside[i].key.Equals("Events"))
					{
						continue;
					}
					Scalar text = (sequence.nodesInside[i] as Mapping).GetText("Name");
					if (text == null)
					{
						Debug.LogError("Cant parse event name!!!");
						continue;
					}
					Mapping mapping2 = (sequence.nodesInside[i] as Mapping).GetMapping("Params");
					if (mapping2 == null)
					{
						Debug.LogError("Cant parse event parameters!!!");
						continue;
					}
					ETriggerEvents eTriggerEvents = ETriggerEvents.EVENT_NONE;
					eTriggerEvents = TriggerEvent.GetEventTypeByName(text.text);
					_eventsSettings[i] = new EventSettings(eTriggerEvents);
					foreach (Node item in mapping2)
					{
						_eventsSettings[i].properties.Add(item.key, (item as Scalar).text);
					}
				}
			}
			sequence = mapping.GetSequence("Actions");
			if (sequence != null)
			{
				foreach (Mapping item2 in sequence.nodesInside)
				{
					string text2 = item2.GetText("Name").value.ToString();
					switch (text2)
					{
					case "WallReaction":
						_fightSettings.Add(text2, new WallReactionParams(item2.GetMapping("Params")));
						break;
					case "Disarm":
						_fightSettings.Add(text2, new DisarmTrajectory(item2.GetMapping("Params")));
						break;
					case "HotGround":
						_fightSettings.Add(text2, new HotGroundParams(item2.GetMapping("Params")));
						break;
					}
				}
			}
			Mapping mapping4 = mapping.GetMapping("ShadowForm");
			shadowFormParams = new ShadowFormParams(mapping4);
			_fightSettings.Add("ShadowForm", shadowFormParams);
			wallRepulsionCoefficient = ((WallReactionParams)GetParamsByName("WallReaction")).coefficient;
			sequence = mapping.GetSequence("DefaultItems");
			defaultEquipments = new Equipment[sequence.nodesInside.Count];
			for (int j = 0; j < sequence.nodesInside.Count; j++)
			{
				defaultEquipments[j] = Equipment.Create((Mapping)sequence.nodesInside[j]);
			}
			Scalar text3 = mapping.GetText("DefaultSkeleton");
			if (text3 == null)
			{
				Debug.LogError("Cant find default Skeleton value in FightSettings");
			}
			defaultSkeleton = text3.text;
			sequence = mapping.GetSequence("DefaultTags");
			if (sequence == null)
			{
				Debug.LogError("Cant find default Tags values in FightSettings");
			}
			defaultTags = new string[sequence.nodesInside.Count];
			for (int k = 0; k < sequence.nodesInside.Count; k++)
			{
				defaultTags[k] = ((Scalar)sequence.nodesInside[k]).text;
			}
			text3 = mapping.GetText("DefaultStageIcon");
			if (text3 != null)
			{
				defaultStageIcon = text3.text;
			}
		}

		public static string GetEventProperty(ETriggerEvents type, string propertyKey)
		{
			EventSettings[] eventsSettings = _eventsSettings;
			foreach (EventSettings eventSettings in eventsSettings)
			{
				if (eventSettings.type == type)
				{
					return eventSettings.properties[propertyKey];
				}
			}
			return null;
		}

		public static FightParamsBase GetParamsByName(string effectName)
		{
			if (_fightSettings.ContainsKey(effectName))
			{
				return _fightSettings[effectName];
			}
			return null;
		}
	}
}
