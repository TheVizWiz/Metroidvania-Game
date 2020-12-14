using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Object = System.Object;

public static class SaveManager {
    
    private static BinaryFormatter formatter;

    static SaveManager() {
        formatter = new BinaryFormatter();
    }
    
    public static bool SaveObject(string path, SaveObject o) {
        FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, path), FileMode.OpenOrCreate);
        formatter.Serialize(stream, o);
        return true;
    }

    public static SaveObject LoadObject(string path) {
        FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, path), FileMode.Open);
        if (!stream.CanRead) return null;
        return formatter.Deserialize(stream) as SaveObject;
    }
    

}

[Serializable]
public class SaveObject {
    private Dictionary<string, object> dictionary;

    public SaveObject() {
        dictionary = new Dictionary<string, object>();
    }

    public void Add(string s, Object o) {
        dictionary.Add(s, o);
    }

    public Object Get(string s) {
        return dictionary[s];
    }
    
    public Dictionary<string, object> GetDictionary() {
        return dictionary;
    }
}
