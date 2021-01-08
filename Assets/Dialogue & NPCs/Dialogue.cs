using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Dialogue {
    
    private Queue<string> lines;
    private Queue<string> staticLines;
    private List<DialogueOption> options;
    private Dictionary<string, int> requirements;

    public Dialogue() {
        lines = new Queue<string>();
        staticLines = new Queue<string>();
        options = new List<DialogueOption>();
        requirements = new Dictionary<string, int>();
    }

    public string GetNextLine() {
        return lines.Dequeue();
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
            lines.Enqueue(s);
        }
    }

    [ItemCanBeNull] public List<DialogueOption> Options => options;

    public bool EndDialogue(string optionPicked) {
        foreach (DialogueOption option in options) {
            if (option.displayString == optionPicked) {
                Queue<string> strings = option.pickStrings;
                foreach (string s in strings) {
                    this.lines.Enqueue(s);
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
    public void LoadDialogue(string[] textLines, ref int currentLine) {
        lines = new Queue<string>();
        string[] array = textLines[currentLine].Split(new[] {' '}, 4);
        int numReqs = int.Parse(array[0]);
        int numOptions = int.Parse(array[1]);
        for (int i = 0; i < numReqs; i++) {
            string[] item = textLines[++currentLine].Split(new[] {' '}, 2);
            requirements.Add(item[1], int.Parse(item[0]));
        }
        
        array = textLines[++currentLine].Split('$');
        foreach (string s in array) {
            staticLines.Enqueue(s);
            lines.Enqueue(s);
        }

        ++currentLine;
        for (int i = 0; i < numOptions; i++) {
            options.Add(DialogueOption.CreateOption(textLines, ref currentLine));
        }
    }

    public bool CheckReqs() {
        foreach (KeyValuePair<string, int> req in requirements) {
            if (!Inventory.HasValue(req.Key, req.Value)) return false;
        }

        return true;
    }
}

public class DialogueOption {

    public Queue<string> pickStrings;
    public string displayString;
    private List<InventoryItem> additionItems;
    private List<InventoryItem> removeItems;

    public static DialogueOption CreateOption(string[] file, ref int currentLine) {
        string[] firstLine = file[currentLine].Split(new[] {' '}, 3);
        int numAdditions = int.Parse(firstLine[0]);
        int numRemovals = int.Parse(firstLine[1]);
        DialogueOption option = new DialogueOption {displayString = firstLine[2]};

        for (int i = 0; i < numAdditions; i++) {
            string[] line = file[++currentLine].Split(new[] {' '}, 2);
            option.additionItems.Add(new InventoryItem(line[1], int.Parse(line[0])));
        }
        for (int i = 0; i < numRemovals; i++) {
            string[] line = file[++currentLine].Split(new[] {' '}, 2);
            option.removeItems.Add(new InventoryItem(line[1], int.Parse(line[0])));
        }

        string[] endLine = file[++currentLine].Split('$');
        for (int i = 0; i < endLine.Length; i++) {
            option.pickStrings.Enqueue(endLine[i]);
        }

        ++currentLine;
        return option;
    }

    public DialogueOption() {
        additionItems = new List<InventoryItem>();
        removeItems = new List<InventoryItem>();
        pickStrings = new Queue<string>();
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

    public Queue<string> GetEndStrings() {
        return pickStrings;
    }
}


