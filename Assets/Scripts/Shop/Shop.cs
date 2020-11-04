using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour {
    private int selectedItemId;

    // Selecting category buttons
    public Button puckBtn;
    public Button stadiumBtn;
    public Button slingshotBtn;

    // Selecting item buttons
    public GameObject puckSelectionBtns;
    public GameObject stadiumSelectionBtns;
    public GameObject slingshotSelectionBtns;

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
    }

    public void SelectCategory(int cat) {
        if (cat == 0) {
            // Selecting pucks
            itemImg.sprite = puckBaseSprite;
            puckBtn.interactable = false;
            puckSelectionBtns.SetActive(true);

            stadiumBtn.interactable = true;
            stadiumSelectionBtns.SetActive(false);

            slingshotBtn.interactable = true;
            slingshotSelectionBtns.SetActive(false);
        } else if (cat == 1) {
            // Selecting stadium
            itemImg.sprite = stadiumBaseSprite;
            puckBtn.interactable = true;
            puckSelectionBtns.SetActive(false);

            stadiumBtn.interactable = false;
            stadiumSelectionBtns.SetActive(true);

            slingshotBtn.interactable = true;
            slingshotSelectionBtns.SetActive(false);
        } else {
            // Selecting slingshots
            itemImg.sprite = slingshotBaseSprite;
            puckBtn.interactable = true;
            puckSelectionBtns.SetActive(false);
            
            stadiumBtn.interactable = true;
            stadiumSelectionBtns.SetActive(false);

            slingshotBtn.interactable = false;
            slingshotSelectionBtns.SetActive(true);
        }
        SelectItem(0);
    }

    public void SelectItem(int id) {
        // Select item by id
        selectedItemId = id;
        UpdateSelectedItemUI();
    }

    public void BuyItem() {
        // Buy item
        ItemDb.Instance.UnlockItem(selectedItemId);
        SelectItem(selectedItemId);
    }

    public void EquipItem() {
        // Equip this item
        ItemDb.Instance.EquipItem(selectedItemId);
        UpdateSelectedItemUI();
    }

    private void UpdateSelectedItemUI() {
        itemNameTxt.text = ItemDb.Instance.Items[selectedItemId].name;
        itemImg.sprite = ItemDb.Instance.Items[selectedItemId].sprite;

        if (ItemDb.Instance.IsEquipped(selectedItemId)) {
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
            buyBtn.SetActive(true);
            selectedTxt.SetActive(false);
        }
    }
}
