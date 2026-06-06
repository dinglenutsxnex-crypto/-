using System.Collections.Generic;

public interface ICounter
{
	double GetValue(string key);

	List<string> GetKeyList();

	void Set(string key, double value, bool rewrite = false);

	void SetAll(double value, bool rewrite = false);

	void SetAllDefault();

	void Increment(string key);

	void Increment(string key, double value);

	void Decrement(string key);

	void Decrement(string key, double value);

	void PrintAllData(string headSentence);

	void Clear();
}
