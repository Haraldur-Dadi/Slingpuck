using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager {
    #region Items
    public static void SaveItemsInfo() {
        ItemsInfo data = new ItemsInfo();
        SaveToFile("/items.inf", data);
    }
    public static ItemsInfo LoadItemsInfo() {
        return LoadFromFile("/items.inf") as ItemsInfo;
    }
    #endregion

    #region Currency
    public static void SavePlayerInfo() {
        PlayerInfo data = new PlayerInfo();
        SaveToFile("/player.inf", data);
    }
    public static PlayerInfo LoadPlayerInfo() {
        return LoadFromFile("/player.inf") as PlayerInfo;
    }
    #endregion

    private static void SaveToFile(string file, object data) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + file;
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
        formatter.Serialize(fs, data);
        fs.Close();
    }
    private static object LoadFromFile(string file) {
        string path = Application.persistentDataPath + file;
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);
            object data = formatter.Deserialize(fs);
            fs.Close();
            return data;
        }
        return null;
    } 
}
