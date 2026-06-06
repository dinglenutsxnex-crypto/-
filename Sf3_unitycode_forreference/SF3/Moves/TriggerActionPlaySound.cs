using Nekki.Yaml;
using SF3.Audio;
using UnityEngine;

namespace SF3.Moves
{
	public class TriggerActionPlaySound : TriggerAction
	{
		private readonly string[] _sounds;

		public TriggerActionPlaySound(Node yamlNode)
			: base(EActionType.PLAY_SOUND, yamlNode)
		{
			Sequence sequence = BaseMapping.GetSequence("Sounds");
			if (sequence != null)
			{
				_sounds = new string[sequence.nodesInside.Count];
				for (int i = 0; i < sequence.nodesInside.Count; i++)
				{
					_sounds[i] = sequence.nodesInside[i].value.ToString();
				}
			}
		}

		protected override void ApplyAction(object modelData)
		{
			base.ApplyAction(modelData);
			if (_sounds != null && _sounds.Length > 0)
			{
				AudioManager.Instance.PlaySound(_sounds[Random.Range(0, _sounds.Length)]);
			}
		}
	}
}
