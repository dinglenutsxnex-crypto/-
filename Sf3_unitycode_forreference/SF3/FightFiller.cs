using Nekki.UI;
using sf3DTO;

namespace SF3
{
	public class FightFiller : HeaderFiller
	{
		protected override bool IsEqualConditions(FightResult result)
		{
			return result.BattleType != sf3DTO.BattleType.Survival;
		}

		protected override void Fill(FightResult result, NekkiUILabel header)
		{
			sf3DTO.FightResult resultType = result.resultType;
			if (resultType == sf3DTO.FightResult.Win)
			{
				SetAlias("congratulations");
			}
			else
			{
				SetAlias("loss");
			}
		}
	}
}
