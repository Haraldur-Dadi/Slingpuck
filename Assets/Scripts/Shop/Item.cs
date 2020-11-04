using UnityEngine;

public class Item {
    public string name;
    public bool isUnlocked;
    public float cost;
    public Sprite sprite;

    public Item (string name, float cost) {
        this.name = name;
        this.cost = cost;
        this.sprite = Resources.Load<Sprite>(name);
    }
}