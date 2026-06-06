using SF3.UserData;
using UnityEngine;
using sf3DTO;

public class CurrenciesTextController : MonoBehaviour
{
	public UILabel coinsBalanceLbl;

	public UILabel bonusBalanceLbl;

	private void Awake()
	{
		UserManager.AddActionForCurrency(CurrencyType.Coin, UpdateCoinsLbl);
		UserManager.AddActionForCurrency(CurrencyType.Bonus, UpdateBonusLbl);
	}

	private void Start()
	{
		UpdateCoinsLbl(UserManager.GetCurrencyValue(CurrencyType.Coin));
		UpdateBonusLbl(UserManager.GetCurrencyValue(CurrencyType.Bonus));
	}

	private void UpdateCoinsLbl(long coins)
	{
		coinsBalanceLbl.text = coins.ToString();
	}

	private void UpdateBonusLbl(long bonus)
	{
		bonusBalanceLbl.text = bonus.ToString();
	}

	private void OnDestroy()
	{
		UserManager.RemoveActionForCurrency(CurrencyType.Coin, UpdateCoinsLbl);
		UserManager.RemoveActionForCurrency(CurrencyType.Bonus, UpdateBonusLbl);
	}
}
