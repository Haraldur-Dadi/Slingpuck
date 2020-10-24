using System.Collections.Generic;
using UnityEngine;

public class ItemDb : MonoBehaviour {
    public static ItemDb Instance;
    public List<Item> Pucks;
    public List<Item> Stadiums;
    public List<Item> Slingshots;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }

        BuildItemDb();
    }

    public Item GetItem(int ID) {
        return Pucks[ID];
    }

    public int GetLengthOfCat() {
        return Pucks.Count;
    }

    void BuildItemDb() {
        Pucks = new List<Item>() {
            new Item("Normal", 0, 0, 0),
            new Item("Next", 1, 0, 100),
            new Item("3", 2, 0, 50),
            new Item("4", 3, 0, 250)
        };
    }
}
