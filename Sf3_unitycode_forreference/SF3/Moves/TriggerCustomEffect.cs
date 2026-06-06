using System;
using Nekki.Yaml;
using SF3.Effects;
using SF3.GameModels;
using UnityEngine;

namespace SF3.Moves
{
	public class TriggerCustomEffect : TriggerAction
	{
		public bool attachToLocal { get; private set; }

		public string attachToBone { get; private set; }

		public bool looped { get; private set; }

		public bool follow { get; private set; }

		public Vector3 shift { get; private set; }

		public Vector3 followAxis { get; private set; }

		public int poolQuantity { get; private set; }

		public bool singleInstance { get; private set; }

		public TriggerCustomEffect(Node yamlNode)
			: base(EActionType.CUSTOM_EFFECT, yamlNode)
		{
			if (yamlNode is Scalar)
			{
				throw new Exception(string.Format("Action CustomEffect can not be Scalar"));
			}
			Mapping mapping = (Mapping)((Mapping)yamlNode).nodesInside[0];
			shift = Vector3.zero;
			Scalar text = mapping.GetText("AttachToBone");
			if (text != null)
			{
				attachToBone = text.text;
			}
			text = mapping.GetText("Looped");
			if (text != null)
			{
				looped = text.text == "1";
			}
			follow = false;
			followAxis = Vector3.zero;
			text = mapping.GetText("Follow");
			if (text != null)
			{
				follow = text.text == "1";
				int num = (follow ? 1 : 0);
				followAxis = new Vector3(num, num, num);
			}
			Mapping mapping2 = mapping.GetMapping("Shift");
			if (mapping2 != null)
			{
				float x = Convert.ToSingle(mapping2.GetText("X").text);
				float y = Convert.ToSingle(mapping2.GetText("Y").text);
				float z = Convert.ToSingle(mapping2.GetText("Z").text);
				shift = new Vector3(x, y, z);
			}
			mapping2 = mapping.GetMapping("Follow");
			if (mapping2 != null)
			{
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				Scalar text2 = mapping2.GetText("X");
				if (text2 != null)
				{
					num2 = Convert.ToSingle(text2.text);
				}
				text2 = mapping2.GetText("Y");
				if (text2 != null)
				{
					num3 = Convert.ToSingle(text2.text);
				}
				text2 = mapping2.GetText("Z");
				if (text2 != null)
				{
					num4 = Convert.ToSingle(text2.text);
				}
				if (num2 == 1f || num3 == 1f || num4 == 1f)
				{
					follow = true;
				}
				followAxis = new Vector3(num2, num3, num4);
			}
			poolQuantity = 0;
			text = mapping.GetText("PoolQuantity");
			if (text != null)
			{
				poolQuantity = int.Parse(text.text);
			}
			singleInstance = false;
			text = mapping.GetText("SingleInstance");
			if (text != null)
			{
				singleInstance = text.text.Equals("1");
			}
			attachToLocal = false;
			text = mapping.GetText("AttachToLocal");
			if (text != null)
			{
				attachToLocal = text.text.Equals("1");
			}
		}

		protected override void ApplyAction(object modelData)
		{
			if (!Model.disableEffects)
			{
				Vector3 vector = new Vector3(0f, 90f, 0f);
				vector.x = Vector2.Angle(Vector2.right, Model.hitResult.Impulse);
				Vector3 angle = vector;
				EffectsManager.PlayCustomEffect((Model)modelData, base.name, angle, this);
			}
		}
	}
}
