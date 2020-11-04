using System.Collections.Generic;

[System.Serializable]
public class ItemsInfo {
    private List<int> isUnlocked;
    public int[] unlocked;
    public int[] equippedItems;

    public ItemsInfo () {
        isUnlocked = new List<int>();
        foreach (Item item in ItemDb.Instance.Items) {
            if (item.isUnlocked)
                isUnlocked.Add(item.id);
        }
        int index = 0;
        unlocked = new int[isUnlocked.Count];
        foreach (int id in isUnlocked) {
            unlocked[index] = id;
            index++;
        }

        equippedItems = ItemDb.Instance.EquippedItems;
    }
}