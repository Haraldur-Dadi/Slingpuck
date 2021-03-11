using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour {
    private int catId;
    private int selectedItemId;

    // Selecting category buttons
    public Button puckBtn;
    public Button stadiumBtn;

    // Selecting item buttons
    public Transform itemSelectParent;
    public GameObject itemSelectBtn;

    // Base sprites for category item image
    public Sprite puckBaseSprite;
    public Sprite stadiumBaseSprite;
    public Sprite slingshotBaseSprite;

    // UI elements
    public GameObject buyBtn;
    public TextMeshProUGUI buyAmountTxt;
    public GameObject equipBtn;
    public GameObject selectedTxt;
    public Image itemImg;
    public TextMeshProUGUI itemNameTxt;

    private void Start() {
        SelectCategory(0);
        puckBtn.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
        stadiumBtn.onClick.AddListener(() => AudioManager.Instance.PlayButtonClick());
    }

    public void SelectCategory(int cat) {
        catId = cat;
        if (cat == 0) {
            // Selecting pucks
            itemImg.sprite = puckBaseSprite;
            puckBtn.interactable = false;
            stadiumBtn.interactable = true;
        } else if (cat == 1) {
            // Selecting stadium
            itemImg.sprite = stadiumBaseSprite;
            puckBtn.interactable = true;
            stadiumBtn.interactable = false;
        } else {
            // Selecting slingshots
            itemImg.sprite = slingshotBaseSprite;
            puckBtn.interactable = true;
            stadiumBtn.interactable = true;
        }
        CreateSelectItemButtons();
    }
    private void CreateSelectItemButtons() {
        // Remove all previous buttons
        int childs = itemSelectParent.childCount;
        for (int i = childs-1; i>=0; i--) {
            Destroy(itemSelectParent.GetChild(i).gameObject);
        }
        // Instantiate new buttons
        int[] indexes = ItemDb.Instance.CategoryIndexes(catId);
        for (int i = indexes[0]; i < indexes[1]; i++) {
            InstatiateSelectBtn(i);
        }
        SelectItem(indexes[0]);
    }
    private void InstatiateSelectBtn(int id) {
        // Create new button
        GameObject go = Instantiate(itemSelectBtn, itemSelectParent);
        // Add listener
        var button = go.GetComponent<Button>();
        button.onClick.AddListener(() => {
            AudioManager.Instance.PlayButtonClick();
            SelectItem(id);
        });
        // Change sprite
        var img = go.GetComponent<Image>();
        img.sprite = ItemDb.Instance.Items[id].sprite;
    }

    public void SelectItem(int id) {
        // Select item by id
        selectedItemId = id;
        UpdateSelectedItemUI();
    }
    public void BuyItem() {
        // Buy item
        AudioManager.Instance.PlayButtonClick();
        CurrencyManager.Instance.SubtractGold(ItemDb.Instance.Items[selectedItemId].cost);
        ItemDb.Instance.UnlockItem(selectedItemId);
        SelectItem(selectedItemId);
    }
    public void EquipItem() {
        // Equip this item
        AudioManager.Instance.PlayButtonClick();
        ItemDb.Instance.EquipItem(catId, selectedItemId);
        UpdateSelectedItemUI();
    }

    public void UpdateSelectedItemUI() {
        itemNameTxt.text = ItemDb.Instance.Items[selectedItemId].name;
        itemImg.sprite = ItemDb.Instance.Items[selectedItemId].sprite;
        if (catId == 0) {
            itemImg.rectTransform.sizeDelta = new Vector2(300, 300);
        } else if (catId == 1) {
            itemImg.rectTransform.sizeDelta = new Vector2(300, 600);
        }

        if (ItemDb.Instance.IsEquipped(catId, selectedItemId)) {
            equipBtn.SetActive(false);
            buyBtn.SetActive(false);
            selectedTxt.SetActive(true);
        } else if (ItemDb.Instance.Items[selectedItemId].isUnlocked) {
            equipBtn.SetActive(true);
            buyBtn.SetActive(false);
            selectedTxt.SetActive(false);
        } else {
            buyAmountTxt.text = ItemDb.Instance.Items[selectedItemId].cost.ToString();
            equipBtn.SetActive(false);
            buyBtn.GetComponent<Button>().interactable = CurrencyManager.Instance.CanBuy(ItemDb.Instance.Items[selectedItemId].cost);
            buyBtn.SetActive(true);
            selectedTxt.SetActive(false);
        }
    }
}
