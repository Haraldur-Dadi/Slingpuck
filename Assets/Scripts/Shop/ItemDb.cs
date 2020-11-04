using System.Collections.Generic;
using UnityEngine;

public class ItemDb : MonoBehaviour {
    public static ItemDb Instance;
    public List<Item> Items;
    public int[] EquippedItems; // Puck, Stadium, Slingshot
    private int PucksLength = 4;
    private int StadiumsLength = 3;
    private int SlingshotsLength = 1;

    public SpriteRenderer Board;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            StadiumsLength += PucksLength;
            SlingshotsLength += StadiumsLength;
            BuildItemDb();
            LoadData();
            SetBoardSprite();
        } else {
            Destroy(this);
        }
    }

    public Sprite GetEquippedPuck() {
        int index = EquippedItems[0];
        return Items[index].sprite;
    }
    public void UnlockItem(int id) {
        Items[id].isUnlocked = true;
        SaveManager.SaveItemsInfo();
    }
    public void EquipItem(int cat, int id) {
        EquippedItems[cat] = id;
        if (cat == 1) {
            SetBoardSprite();
        }
        SaveManager.SaveItemsInfo();
    }
    private void SetBoardSprite() {
        int id = EquippedItems[1];
        Board.sprite = Items[id].sprite;
    }
    public bool IsEquipped (int cat, int id) {
        return EquippedItems[cat] == id;
    }
    public int[] CategoryIndexes(int cat) {
        // Return start and end indexes of category
        int[] i = new int[2];
        if (cat == 0) {
            // return for Puck
            i[0] = 0;
            i[1] = PucksLength;
        } else if (cat == 1) {
            // return for Stadiums
            i[0] = PucksLength;
            i[1] = StadiumsLength;
        } else {
            // return for Slingshots
            i[0] = StadiumsLength;
            i[1] = SlingshotsLength;
        }
        return i;
    }

    private void BuildItemDb() {
        Items = new List<Item>() {
            // Pucks
            new Item("Puck", 0),
            new Item("Ice Hockey Puck", 100),
            new Item("Football", 50),
            new Item("Basketball", 250),
            // Stadiums
            new Item("Stadium", 0),
            new Item("Ice Hockey Rink", 350),
            new Item("Football Stadium", 500),
            // Slingshots
            new Item("Slingshot", 0)
        };
    }
    private void LoadData() {
        // Load Data
        ItemsInfo info = SaveManager.LoadItemsInfo();
        // Set info
            // Unlock items
        for (int i = 0; i < Items.Count; i++) {
            if (info == null) {
                // Auto unlock "standard" items
                if (i == 0 || i == PucksLength || i == StadiumsLength) {
                    Items[i].isUnlocked = true;
                } else {
                    Items[i].isUnlocked = false;
                }
            } else {
                Items[i].isUnlocked = info.unlocked[i];
            }
        }
            // Equip items
        if (info == null) {
            // Equip standard items
            EquippedItems = new int[]{0, PucksLength, StadiumsLength};
        } else {
            EquippedItems = info.equippedItems;
        }
    }
}
