namespace SF3.Tactics.Analytics
{
	public class ResultInfo
	{
		public const string CSV_HEADER = "Provocation,Reaction,Distance,StartFrame,ZeroFrame,RealHitFrame,CalcHitFrame,DamageDone,Result,DownFrames,RealRepulsion,CalcRepulsion\n";

		public string provocation;

		public string reaction;

		public bool provocationIsAttack;

		public bool reactionIsAttack;

		public float distance;

		public float startFrame;

		public float zeroFrame;

		public float damageDone;

		public ResultType type;

		public int downFrames;

		public float realRepulsion;

		public float calcRepulsion;

		public float realHitFrame;

		public float calcHitFrame;

		public override string ToString()
		{
			return "[" + provocation + "][" + reaction + "] distance: " + distance + " startFrame " + startFrame + " damageDone: " + damageDone + " Result: " + type;
		}

		public string ToCSVRow()
		{
			return string.Concat(provocation, ",", reaction, ",", distance, ",", startFrame, ",", zeroFrame, ",", realHitFrame, ",", calcHitFrame, ",", damageDone, ",", type, ",", downFrames, ",", realRepulsion, ",", calcRepulsion);
		}
	}
}
