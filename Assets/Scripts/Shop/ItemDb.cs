using System.Collections.Generic;
using UnityEngine;

public class ItemDb : MonoBehaviour {
    public static ItemDb Instance;
    public List<Item> Items;
    public int[] EquippedItems; // Puck, Stadium, Slingshot
    private int PucksLength = 7;
    private int StadiumsLength = 5;
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
        int id = EquippedItems[0];
        Item item = Items.Find(i => i.id == id);
        return item.sprite;
    }
    public void UnlockItem(int id) {
        Items[id].isUnlocked = true;
        SaveManager.SaveItemsInfo();
    }
    public void EquipItem(int cat, int id) {
        EquippedItems[cat] = Items[id].id;
        if (cat == 0) {
            GameManager.Instance.ChangePuckAppearance(Items[id].sprite);
        } else if (cat == 1) {
            SetBoardSprite();
        }
        SaveManager.SaveItemsInfo();
    }
    private void SetBoardSprite() {
        int id = EquippedItems[1];
        Item item = Items.Find(i => i.id == id);
        Board.sprite = item.sprite;
    }
    public bool IsEquipped(int cat, int id) {
        return EquippedItems[cat] == Items[id].id;
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
            new Item(0, "Puck", 0), // Free
            new Item(1, "Ice Hockey Puck", 150),
            new Item(2, "Football", 150),
            new Item(3, "Basketball", 200),
            new Item(9, "Christmas Ball", 200), // Free for logging in during christmas event
            new Item(10, "Candy Cane", 250),
            new Item(11, "Christmas Wreath", 300),
            new Item(13, "Tennis Ball", 200),
            new Item(14, "Juggle Ball", 200),
            // Stadiums
            new Item(4, "Stadium", 0), // Free
            new Item(5, "Ice Hockey Rink", 250),
            new Item(6, "Football Stadium", 275),
            new Item(8, "Basketball Court", 400),
            new Item(12, "Christmas Park", 350),
            new Item(15, "Tennis Court", 300),
            // Slingshots
            new Item(7, "Slingshot", 0) // Free
        };
    }
    private void LoadData() {
        // Load Data
        ItemsInfo info = SaveManager.LoadItemsInfo();
        // Set info
        if (info == null) {
            // Auto unlock "standard" items and equip them
            Items[0].isUnlocked = Items[PucksLength].isUnlocked = Items[StadiumsLength].isUnlocked = true;
            EquippedItems = new int[]{0, 4, 7};
        } else {
            // Unlock items
            foreach (int id in info.unlocked) {
                Item item = Items.Find(i => i.id == id);
                item.isUnlocked = true;
            }
            // Equip items
            EquippedItems = new int[3];
            for (int x = 0; x < info.equippedItems.Length; x++) {
                Item item = Items.Find(i => i.id == info.equippedItems[x]);
                EquippedItems[x] = item.id;
            }
        }
    }
}
