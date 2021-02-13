using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

[Serializable]
public class Dialogue {
    
    public List<string> lines;
    public List<string> staticLines;
    public List<DialogueOption> options;
    // public Dictionary<string, int> requirements;
    public List<InventoryItem> requirements;

    private string currentLine;

    public Dialogue() {
        lines = new List<string>();
        staticLines = new List<string>();
        options = new List<DialogueOption>();
        requirements = new List<InventoryItem>();
    }

    public string GetNextLine() {
        string s = lines[0];
        lines.RemoveAt(0);
        currentLine = s;
        return s;
    }

    public string GetCurrentLine() {
        return currentLine;
    }

    public bool HasNextLine() {
        return lines.Count > 0;
    }

    public bool IsLastLine() {
        return lines.Count == 0;
    }

    public void ClearLines() {
        lines.Clear();
    }

    public void ResetLines() {
        lines.Clear();
        foreach (string s in staticLines) {
            lines.Add(s);
        }
    }

    public void InitializeStaticLines() {
        foreach (string s  in lines) {
            staticLines.Add(s);
        }
    }


    public bool EndDialogue(string optionPicked) {
        foreach (DialogueOption option in options) {
            if (option.displayString == optionPicked) {
                List<string> strings = option.pickStrings;
                foreach (string s in strings) {
                    this.lines.Add(s);
                }
                return option.Finish();
            }
        }
        return false;
    }

    /// <summary>
    /// Loads Dialogue from text file
    /// Line 1, 2, 3: name, description, # of each: requirements, removals, additions, end options.
    /// each addition, removal, and requirement consists of an amount and then a name for the given inventoryItem.
    /// each option consists of a name for the option, followed by how much ever the option costs.
    /// an option can only cost in a single currency, not multiple different currencies. String for a given option will be added after the option.
    /// </summary>
    /// <param name="textLines"></param>
    /// <param name="start"></param>

    public bool CheckReqs() {
        foreach (InventoryItem item in requirements) {
            if (!Inventory.HasValue(item.name, item.amount)) return false;
        }

        return true;
    }
}

[Serializable]
public class DialogueOption {

    public List<string> pickStrings;
    public string displayString;
    public List<InventoryItem> additionItems;
    public List<InventoryItem> removeItems;

    public DialogueOption() {
        additionItems = new List<InventoryItem>();
        removeItems = new List<InventoryItem>();
        pickStrings = new List<string>();
    }

    public bool Finish() {
        foreach (InventoryItem item in removeItems) {
            if (!Inventory.HasValue(item.name, item.amount)) return false;
        }

        foreach (InventoryItem item in removeItems) {
            Inventory.Discard(item.name, item.amount);
        }
        
        foreach (InventoryItem item in additionItems) {
            Inventory.PickUp(item.name, item.amount);
        }

        return true;
    }

    public List<string> GetEndStrings() {
        return pickStrings;
    }
}


