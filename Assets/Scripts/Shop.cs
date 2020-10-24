using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour {
    private Item selectedItem;
    public TextMeshProUGUI itemNameTxt;
    public Image itemImg;

    public Button buyBtn;
    public TextMeshProUGUI buyAmountTxt;
    public Button equipBtn;
    public TextMeshProUGUI equipTxt;
    public GameObject unlock;

    private void Start() {
        SelectItem(0);
    }

    public void SelectItem(int id) {
        selectedItem = ItemDb.Instance.GetItem(id);
        UpdateSelectedUI();
    }

    private void UpdateSelectedUI() {
        itemNameTxt.text = selectedItem.name;
        itemImg.material = selectedItem.material;
    }

    public void BuyItem() {
    }

    public void EquipItem() {
    }
}
