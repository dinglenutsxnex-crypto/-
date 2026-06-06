using sf3DTO;

public interface ICurrencyOperations
{
	Currency GetCurrency(CurrencyType type);

	long GetCurrencyValue(CurrencyType type);

	void SetCurrency(Currency c);

	void SetCurrency(CurrencyType type, long value);

	void AddCurrecy(CurrencyType type, long value);

	void AddCurrecy(Currency c);

	void SubtractCurrency(CurrencyType type, long value);

	void SubtractCurrency(Currency c);

	bool HasAnyCurrency();
}
