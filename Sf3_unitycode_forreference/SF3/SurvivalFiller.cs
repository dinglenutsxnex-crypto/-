using Nekki.UI;
using sf3DTO;

namespace SF3
{
	public class SurvivalFiller : HeaderFiller
	{
		protected override bool IsEqualConditions(FightResult result)
		{
			return result.BattleType == sf3DTO.BattleType.Survival;
		}

		protected override void Fill(FightResult result, NekkiUILabel header)
		{
			SetAlias("survival_battle_finished");
			SetFormat(result.roundsWon);
		}
	}
}
