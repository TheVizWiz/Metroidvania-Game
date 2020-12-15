using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor.Presets;
using UnityEngine;

public class Inventory {

    private static Dictionary<string, int> items;
    private static SaveObject saveObject;
    private static string PATH = "playerinventory";

    static Inventory() {
        items = new Dictionary<string, int>();
        saveObject = new SaveObject();
    }

    public void PickUp(string item, int amount) {
        items.Add(item, amount);
    }

    public bool Discard(string item, int amount) {
        if (!HasValue(item, amount)) return false;
        
        if (items[item] == amount) 
            items.Remove(item);
        else 
            items[item] = items[item] - amount;
        return true;
    }

    public bool HasValue(string item, int amount) {
        
        int currentAmount = -1;
        bool containsItem = items.TryGetValue(item, out currentAmount);
        if (!containsItem) return false;
        if (currentAmount < amount) return false;
        return true;
    }
    

    public void Save() {
        saveObject.AddAll(items);
        SaveManager.SaveObject(Path.Combine(GameManager.GetPath(), PATH), saveObject);
    }

    public void Load() {
        saveObject = SaveManager.LoadObject(Path.Combine(GameManager.GetPath(), PATH));
    }

}
