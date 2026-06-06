using System;
using System.Collections.Generic;
using Nekki.Yaml;
using SimpleJSON;
using sf3DTO;

public interface IBattleInfo : ICloneable
{
	BattleInfo GetBattleInfo();

	int GetID();

	BattleType GetBattleType();

	List<FightInfo> GetFights();

	int GetWonFights();

	void SetBattleHidden(bool value);

	void SetBattleAvailable(bool value);

	Mapping ToYAML();

	JSONClass ToJSON();

	FightInfo GetCurrentFight();

	bool GetIsCompleted();

	bool GetIsHidden();

	bool GetIsAvailable();

	long GetCooldown();

	bool HasCooldown();

	DateTime GetGenerationTime();

	DateTime GetExpirationTime();

	DateTime GetFinishTime();

	void SetExpirationTime();

	bool HasExpirationTime();

	void ClearExpirationTime();

	void CompleteFight(FightResult resultFight);

	void MergeWith(Battle battleValue, int battleIndex);

	int GetBattleCounter();

	void SetTestBattleType();

	int[] GetChapters();

	LocationInfo GetLocation();

	void SetCurrentFight(int fightIndex);
}
