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
    
    /// <summary>
    /// Saves a SaveObject at the given path.
    /// </summary>
    /// <param name="path">path to save the object to, will override files if they already exist</param>
    /// <param name="o">the SaveObject to serialize</param>
    /// <returns>true if the process was completed successfully, false otherwise</returns>
    public static bool SaveObject(string path, SaveObject o) {
        FileStream stream = new FileStream(Path.Combine(Application.persistentDataPath, path), FileMode.OpenOrCreate);
        formatter.Serialize(stream, o);
        return true;
    }

    /// <summary>
    /// deserializes and returns a SaveObject based on given path
    /// </summary>
    /// <param name="path"> path of the item to be gotten</param>
    /// <returns>null if no file is found, otherwise SaveObject with dictionary from file</returns>
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

    public SaveObject(Dictionary<string, object> d) {
        dictionary = new Dictionary<string, object>();
        AddAll(d);

    }
    

    public void Add(string s, object o) {
        dictionary.Add(s, o);
    }

    public object Get(string s) {
        return dictionary[s];
    }

    public void AddAll(Dictionary<string, object> d) {
        foreach (KeyValuePair<string, object> o in d) {
            Add(o.Key, o.Value);
        }
    }
    
    public void AddAll(Dictionary<string, int> d) {
        foreach (KeyValuePair<string, int> o in d) {
            Add(o.Key, o.Value);
        }
    }
    
    
    public Dictionary<string, object> GetDictionary() {
        return dictionary;
    }
}
