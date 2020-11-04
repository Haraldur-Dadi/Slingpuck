using System.Collections.Generic;
using UnityEngine;

public class ItemDb : MonoBehaviour {
    public static ItemDb Instance;
    public List<Item> Items;
    public int[] EquippedItems; // Puck, Stadium, Slingshot
    private int PucksLength = 4;
    private int StadiumLength = 1;

    public SpriteRenderer Board;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            StadiumLength += PucksLength;
            BuildItemDb();
            LoadData();
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
    public void EquipItem(int id) {
        if (id < PucksLength) {
            EquippedItems[0] = id;
        } else if (id < StadiumLength) {
            EquippedItems[1] = id;
            Board.sprite = Items[id].sprite;
        } else {
            EquippedItems[2] = id;
        }
        SaveManager.SaveItemsInfo();
    }
    public bool IsEquipped (int id) {
        for (int i = 0; i < EquippedItems.Length; i++) {
            if (EquippedItems[i] == id)
                return true;
        }
        return false;
    }

    private void BuildItemDb() {
        Items = new List<Item>() {
            // Pucks
            new Item("Puck", 0),
            new Item("Hockey Puck", 100),
            new Item("Football", 50),
            new Item("4", 250),
            // Stadiums
            new Item("Standard", 0),
            // Slingshots
            new Item("Standard", 0)
        };
    }
    private void LoadData() {
        // Load Data
        ItemsInfo info = SaveManager.LoadItemsInfo();
        // Set info
            // Unlock items
        for (int i = 0; i < Items.Count; i++) {
            if (info.unlocked == null) {
                // Auto unlock "standard" items
                if (i == 0 || i == PucksLength || i == StadiumLength) {
                    Items[i].isUnlocked = true;
                } else {
                    Items[i].isUnlocked = false;
                }
            } else {
                Items[i].isUnlocked = info.unlocked[i];
            }
        }
            // Equip items
        if (info.equippedItems == null) {
            // Equip standard items
            EquippedItems = new int[]{0, PucksLength, StadiumLength};
        } else {
            EquippedItems = info.equippedItems;
        }
    }
}
