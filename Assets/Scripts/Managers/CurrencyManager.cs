using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour {
    #region Instance
    public static CurrencyManager Instance;
    private void Awake () {
        if (Instance == null) {
            Instance = this;
            PlayerInfo info = SaveManager.LoadPlayerInfo();
            gold = (info is null) ? 0 : info.currencyAmount;
            goldTxt.text = gold.ToString();
        } else {
            Destroy(gameObject);
        }
    }
    #endregion
    private int gold;
    public TextMeshProUGUI goldTxt;

    public int GoldAmount() {
        return gold;
    }
    public bool CanBuy(int amount) {
        return gold >= amount;
    }
    public void AddGold(int goldToAdd) {
        gold += goldToAdd;
        goldTxt.text = gold.ToString();
        SaveManager.SavePlayerInfo();
        GetComponent<Shop>().UpdateSelectedItemUI();
    }
    public void SubtractGold(int goldToSub) {
        gold -= goldToSub;
        goldTxt.text = gold.ToString();
        SaveManager.SavePlayerInfo();
    }
}
