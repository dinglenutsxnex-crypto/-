using System;
using System.Collections.Generic;
using Nekki;
using SF3;
using sf3DTO;

public class CurrencyHolder : ICurrencyOperations, ICloneable
{
	private Dictionary<CurrencyType, Currency> _currency;

	public CurrencyHolder()
	{
		_currency = new Dictionary<CurrencyType, Currency>();
		foreach (CurrencyType enumerator2 in EnumsCompliancer.GetEnumerators<CurrencyType>())
		{
			Currency value = new Currency
			{
				CurrencyType = enumerator2,
				Value = 0L
			};
			_currency.Add(enumerator2, value);
		}
	}

	public Currency GetCurrency(CurrencyType type)
	{
		return _currency[type];
	}

	public long GetCurrencyValue(CurrencyType type)
	{
		return _currency[type].Value;
	}

	public void SetCurrency(Currency c)
	{
		SetCurrency(c.CurrencyType, c.Value);
	}

	public void SetCurrency(CurrencyType type, long value)
	{
		_currency[type].Value = value;
	}

	public void AddCurrecy(Currency c)
	{
		AddCurrecy(c.CurrencyType, c.Value);
	}

	public void AddCurrecy(CurrencyType type, long value)
	{
		_currency[type].Value += value;
	}

	public void SubtractCurrency(Currency c)
	{
		SubtractCurrency(c.CurrencyType, c.Value);
	}

	public void SubtractCurrency(CurrencyType type, long value)
	{
		AddCurrecy(type, -value);
	}

	public bool HasAnyCurrency()
	{
		foreach (KeyValuePair<CurrencyType, Currency> item in _currency)
		{
			if (item.Value.Value > 0)
			{
				return true;
			}
		}
		return false;
	}

	public object Clone()
	{
		CurrencyHolder currencyHolder = new CurrencyHolder();
		foreach (CurrencyType enumerator2 in EnumsCompliancer.GetEnumerators<CurrencyType>())
		{
			currencyHolder.SetCurrency(enumerator2, GetCurrencyValue(enumerator2));
		}
		return currencyHolder;
	}

	public override string ToString()
	{
		StringWrapper stringWrapper = StringWrapper.Create();
		stringWrapper.Head("CurrencyHolder data");
		foreach (KeyValuePair<CurrencyType, Currency> item in _currency)
		{
			stringWrapper.Wrap(item.Key.ToString(), item.Value.Value);
		}
		stringWrapper.Separator();
		return stringWrapper.ToString();
	}
}
