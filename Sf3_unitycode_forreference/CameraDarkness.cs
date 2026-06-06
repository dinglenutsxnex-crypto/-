using System;
using System.Collections.Generic;
using SF3;
using SF3.Utils;
using UnityEngine;

public class CameraDarkness : MonoBehaviour
{
	public enum Stage
	{
		TransitionToDarkness = 0,
		Darkness = 1,
		TransitionToRadiance = 2,
		Radiance = 3,
		None = 4
	}

	public struct State
	{
		public float Progress;

		public Stage Stage;

		public State(float totalProgress, float darkFrames, float lightFrames, float transitionFrames, float totalFrames)
		{
			float num = darkFrames / totalFrames;
			float num2 = lightFrames / totalFrames;
			float num3 = transitionFrames / totalFrames;
			float num4 = num3;
			if (totalProgress > 0f && totalProgress <= num2)
			{
				Stage = Stage.Radiance;
				Progress = totalProgress / num2;
				return;
			}
			if (totalProgress > num2 && totalProgress <= num2 + num3)
			{
				Stage = Stage.TransitionToDarkness;
				Progress = (totalProgress - num2) / num3;
				return;
			}
			if (totalProgress > num2 + num3 && totalProgress <= num2 + num3 + num)
			{
				Stage = Stage.Darkness;
				Progress = (totalProgress - (num2 + num3)) / num;
				return;
			}
			Stage = Stage.TransitionToRadiance;
			if (Math.Abs(totalProgress) < 0.0001f)
			{
				Progress = 1f;
			}
			else
			{
				Progress = (totalProgress - (num2 + num3 + num)) / num4;
			}
		}
	}

	private static CameraDarkness _instance;

	private Material _mat;

	private bool darknessEnabled;

	private List<BehaviourTimer.SingleTimer> shadowTimers = new List<BehaviourTimer.SingleTimer>();

	private bool terminateDarkness;

	private float darkFrames;

	private float lightFrames;

	private float transitionFrames;

	private float totalFrames;

	public static CameraDarkness Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Camera.main.gameObject.AddComponent<CameraDarkness>();
				_instance._mat = new Material(Shader.Find("Nekki/Darkness"));
				_instance._mat.SetColor("_Color", new Color(0f, 0f, 0f));
			}
			return _instance;
		}
	}

	private void Set(float amount, Stage stage)
	{
		darknessEnabled = amount > 0f;
		Color color = new Color(Mathf.Clamp01(amount), Mathf.Clamp01(amount), Mathf.Clamp01(amount));
		_mat.SetColor("_Color", color);
	}

	public void SetupDarkness(float darkFrames, float lightFrames, float transitionFrames)
	{
		this.darkFrames = darkFrames;
		this.lightFrames = lightFrames;
		this.transitionFrames = transitionFrames;
		totalFrames = darkFrames + lightFrames + transitionFrames * 2f;
		shadowTimers.Add(BehaviourTimer.CreateGameFramesTimer((int)totalFrames, delegate(float progress)
		{
			State state = new State(progress, darkFrames, lightFrames, transitionFrames, totalFrames);
			if (BattleController.Instance.fightController.FightStage != FightController.EFightStage.RoundFightStart)
			{
				SetDarkness(1f);
			}
			else
			{
				switch (state.Stage)
				{
				case Stage.TransitionToDarkness:
					SetDarkness(1f - state.Progress, state.Stage);
					break;
				case Stage.Darkness:
					SetDarkness(0f, state.Stage);
					break;
				case Stage.TransitionToRadiance:
					SetDarkness(state.Progress, state.Stage);
					break;
				case Stage.Radiance:
					SetDarkness(1f, state.Stage);
					break;
				}
			}
		}, this, delegate
		{
			RepeatDarkness();
		}));
	}

	public void StopDarkness()
	{
		foreach (BehaviourTimer.SingleTimer shadowTimer in shadowTimers)
		{
			if (shadowTimer != null)
			{
				shadowTimer.Stop();
			}
		}
		shadowTimers.Clear();
		SetDarkness(1f, Stage.Radiance);
	}

	private void SetDarkness(float amount, Stage stage = Stage.None)
	{
		Set(amount + 0.0001f, stage);
	}

	private void RepeatDarkness()
	{
		if (BattleController.Instance.fightController.FightStage == FightController.EFightStage.RoundFightStart)
		{
			shadowTimers.Clear();
			SetupDarkness(darkFrames, lightFrames, transitionFrames);
		}
	}

	private void OnPostRender()
	{
		if (darknessEnabled)
		{
			GL.PushMatrix();
			GL.LoadOrtho();
			_mat.SetPass(0);
			GL.LoadPixelMatrix();
			GL.Viewport(new Rect(0f, 0f, Screen.width, Screen.height));
			GL.Begin(7);
			GL.Vertex3(0f, 0f, 0f);
			GL.Vertex3(0f, Screen.height, 0f);
			GL.Vertex3(Screen.width, Screen.height, 0f);
			GL.Vertex3(Screen.width, 0f, 0f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
