using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Dialogue {
    
    private Queue<string> lines;
    private Dictionary<string, InventoryItem> options;
    private Dictionary<string, string> finalString;
    private Dictionary<string, int> addItems;
    private Dictionary<string, int> removeItems;
    private Dictionary<string, int> requirements;

    public Dialogue() {
        lines = new Queue<string>();
        options = new Dictionary<string, InventoryItem>();
        addItems = new Dictionary<string, int>();
        removeItems = new Dictionary<string, int>();
        finalString = new Dictionary<string, string>();
        requirements = new Dictionary<string, int>();
    }

    public string GetNextLine() {
        return lines.Dequeue();
    }

    public bool HasNextLine() {
        return lines.Count > 0;
    }

    public bool IsLastLine() {
        return lines.Count == 1;
    }

    public string GetFinalLine(string s) {
        return finalString[s];
    }

    public Dictionary<string, InventoryItem> Options => options;

    public void EndDialogue() {
        foreach (KeyValuePair<string, int> item in addItems) {
            Inventory.PickUp(item.Key, item.Value);
        }

        foreach (KeyValuePair<string, int> item in removeItems) {
            Inventory.Discard(item.Key, item.Value);
        }
    }

    public void CreateDialogue(string[] textLines, ref int start) {
        lines = new Queue<string>();
        string[] array = textLines[start + 0].Split(new[] {' '}, 4);
        int numReqs = int.Parse(array[0]);
        int numRemove = int.Parse(array[1]);
        int numAdd = int.Parse(array[2]);
        int numOptions = int.Parse(array[3]);
        array = textLines[start + 1].Split('$');
        foreach (string s in array) lines.Enqueue(s);
        int currentLine = 2 + start;
        
        for (int i = 0; i < numReqs; i++) {
            string[] item = textLines[++currentLine].Split(new[] {' '}, 2);
            requirements.Add(item[0], int.Parse(item[1]));
        }
        for (int i = 0; i < numRemove; i++) {
            string[] item = textLines[++currentLine].Split(new[] {' '}, 2);
            removeItems.Add(item[0], int.Parse(item[1]));
        }
        for (int i = 0; i < numAdd; i++) {
            string[] item = textLines[++currentLine].Split(new[] {' '}, 2);
            addItems.Add(item[0], int.Parse(item[1]));
        }
        for (int i = 0; i < numOptions; i++) {
            string[] item = textLines[++currentLine].Split(new[] {' '}, 3);
            string s = textLines[currentLine + numOptions];
            options.Add(item[0], new InventoryItem(item[1], int.Parse(item[2])));
            finalString.Add(item[0], s);
        }

        start = currentLine + 1;
    }

    public bool CheckReqs() {
        foreach (KeyValuePair<string, int> req in requirements) {
            if (!Inventory.HasValue(req.Key, req.Value)) return false;
        }

        return true;
    }
}

