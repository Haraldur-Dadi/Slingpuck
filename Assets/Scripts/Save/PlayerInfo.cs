using System.Collections.Generic;

[System.Serializable]
public class PlayerInfo {
    public int currencyAmount;
    public float skillRank;

    public PlayerInfo () {
        currencyAmount = CurrencyManager.Instance.GoldAmount();
        skillRank = Ai.Instance.SkillRank();
    }
}
