using System.Collections.Generic;
using Jint.Native;
using Nekki;
using SF3.Items;
using sf3DTO;

public class ShopBooster
{
	public int ID { get; private set; }

	public string Alias { get; private set; }

	public Dictionary<CurrencyType, long> Currencies { get; private set; }

	public SF3.Items.Booster Booster { get; private set; }

	public ShopBooster(int id, string alias, Dictionary<string, JsValue> currencies)
	{
		Currencies = new Dictionary<CurrencyType, long>();
		foreach (CurrencyType enumerator3 in EnumsCompliancer.GetEnumerators<CurrencyType>())
		{
			int num = (int)enumerator3;
			string key = num.ToString();
			if (currencies.ContainsKey(key))
			{
				Currencies.Add(enumerator3, currencies[key].AsLong());
			}
		}
		ID = id;
		Alias = alias;
		Dictionary<int, SF3.Items.Booster> boosters = JS.Instance.Boosters;
		foreach (KeyValuePair<int, SF3.Items.Booster> item in boosters)
		{
			if (item.Value.model == alias)
			{
				Booster = item.Value;
				break;
			}
		}
	}
}
