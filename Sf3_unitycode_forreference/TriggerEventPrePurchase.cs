using Nekki.Yaml;
using SF3.Items;
using SF3.Moves;
using sf3DTO;

public class TriggerEventPrePurchase : TriggerEvent
{
	private readonly bool _any;

	private readonly CurrencyType _currency;

	public TriggerEventPrePurchase(Mapping eventMap)
		: base(ETriggerEvents.QEVENT_PRE_PURCHASE, eventMap)
	{
		if (eventMap == null)
		{
			_any = true;
			return;
		}
		string outResult;
		TryGetString(out outResult, "Currency", string.Empty, string.Empty, null, false);
		switch (outResult)
		{
		case "coin":
			_currency = CurrencyType.Coin;
			break;
		case "bonus":
			_currency = CurrencyType.Bonus;
			break;
		case "shadow":
			_currency = CurrencyType.Shadow;
			break;
		default:
			_any = true;
			break;
		}
	}

	protected override bool Equal()
	{
		return _any || ShopTransaction.Current.Currency.CurrencyType == _currency;
	}
}
