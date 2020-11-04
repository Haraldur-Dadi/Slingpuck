using System.Collections.Generic;

[System.Serializable]
public class ItemsInfo {
    public bool[] unlocked;
    public int[] equippedItems;

    public ItemsInfo () {
        unlocked = new bool[ItemDb.Instance.Items.Count];
        equippedItems = ItemDb.Instance.EquippedItems;
        
        int index = 0;
        foreach (Item item in ItemDb.Instance.Items) {
            unlocked[index] = ItemDb.Instance.Items[index].isUnlocked;
            index += 1;
        }
    }
}