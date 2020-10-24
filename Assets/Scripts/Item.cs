using UnityEngine;

public class Item {
    /* Constructor class for items that are to be found within the game */

    public string name; // Name of item
    public int ID; // id in catagory
    public int unlockOptions; // 1 = can be bought, 2 = needs to be unlocked, 3 = needs to be bought for real money
    public int cost; // Cost to buy
    public Material material;

    public Item (string name, int id, int unlockOptions, int cost) {
        this.name = name;
        this.ID = id;
        this.unlockOptions = unlockOptions;
        this.cost = cost;
        this.material = Resources.Load<Material>(name);
    }

    public Item (Item item) {
        this.name = item.name;
        this.ID = item.ID;
        this.unlockOptions = item.unlockOptions;
        this.cost = item.cost;
        this.material = item.material;
    }
}