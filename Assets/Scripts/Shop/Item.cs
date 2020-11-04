using UnityEngine;

public class Item {
    public int id;
    public string name;
    public float cost;
    public bool isUnlocked;
    public Sprite sprite;

    public Item (int id, string name, float cost) {
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.isUnlocked = false;
        this.sprite = Resources.Load<Sprite>(name);
    }
}