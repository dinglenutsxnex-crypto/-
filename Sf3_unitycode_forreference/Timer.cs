using System.Diagnostics;

public class Timer
{
	private Stopwatch _sw;

	public double resultDuration;

	public string tag;

	public bool stopped;

	public Timer(string tag)
	{
		this.tag = tag;
		_sw = new Stopwatch();
		Start();
	}

	public void Start()
	{
		_sw.Start();
	}

	public void Stop()
	{
		_sw.Stop();
		resultDuration = _sw.Elapsed.TotalMilliseconds;
	}

	public override string ToString()
	{
		return tag + " : " + resultDuration + " ms";
	}
}
