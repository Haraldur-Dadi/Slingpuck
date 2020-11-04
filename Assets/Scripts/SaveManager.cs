using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager {

    public static void SaveItemsInfo() {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/items.inf";
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
        ItemsInfo data = new ItemsInfo();
        formatter.Serialize(fs, data);
        fs.Close();
    }

    public static ItemsInfo LoadItemsInfo() {
        string path = Application.persistentDataPath + "/items.inf";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            
            FileStream fs = new FileStream(path, FileMode.Open);
            ItemsInfo data = formatter.Deserialize(fs) as ItemsInfo;
            fs.Close();
            
            return data;
        } else {
            return null;
        }
    }
}
