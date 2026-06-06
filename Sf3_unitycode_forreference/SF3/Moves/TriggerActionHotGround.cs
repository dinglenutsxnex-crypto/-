using System.Collections;
using Nekki.Yaml;
using SF3.GameModels;
using SF3.Settings;
using UnityEngine;

namespace SF3.Moves
{
	public class TriggerActionHotGround : TriggerActionRoundResetable
	{
		private class HotGroundRoutine
		{
			private float height;

			private float gameOverTime;

			private float timer;

			private bool onFly;

			private bool run;

			public HotGroundRoutine(float gameOverTime)
			{
				this.gameOverTime = gameOverTime;
				height = ((FightSettings.HotGroundParams)FightSettings.GetParamsByName("HotGround")).height;
				run = true;
			}

			public IEnumerator Launch()
			{
				BattleController.RegisterEventCallback(ETriggerEvents.EVENT_STAGE_CHANGE, delegate(BattleEventArgs e1)
				{
					if ((FightController.EFightStage)e1.EventData == FightController.EFightStage.RoundFightEnd)
					{
						run = false;
					}
				});
				Bone[] bones = ModelsManager.Instance.Player.modelComponents.GetDeepBonesCopy();
				while (run)
				{
					onFly = true;
					Bone[] array = bones;
					foreach (Bone bone in array)
					{
						if (bone.boneID != -1 && bone.transform.position.y - SceneConfig.PointFloor < height)
						{
							onFly = false;
							break;
						}
					}
					if (!onFly)
					{
						timer += GameTimeController.gameTimeDelta;
					}
					else
					{
						timer = 0f;
					}
					Debug.Log(timer + "/" + gameOverTime);
					if (timer > gameOverTime)
					{
						FightController.Instance.WinCurrentRound(ERoundResult.ENEMY_WIN);
						break;
					}
					yield return new WaitForEndOfFrame();
				}
			}
		}

		private float time;

		private Coroutine checker;

		public TriggerActionHotGround(Node yamlNode)
			: base(EActionType.HOT_GROUND, yamlNode)
		{
			TryGetFloat(out time, "Duration", 0f, string.Empty);
		}

		protected override void ApplyAction(object modelData)
		{
			checker = Routiner.Go(new HotGroundRoutine(time).Launch());
		}

		public override void Reset()
		{
			Routiner.Stop(checker);
		}
	}
}
