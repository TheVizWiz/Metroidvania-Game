using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NPC{

    public string name;
    public string description;

    private List<Dialogue> dialogues;
    // Start is called before the first frame update
    public void LoadNPC() {
        string[] file = SaveManager.ReadFileFromResources(name);
        int currentLine = 0;
        while (currentLine < file.Length) {
            Dialogue dialogue = new Dialogue();
            dialogue.CreateDialogue(file, ref currentLine);
            Debug.Log(currentLine);
            dialogues.Add(dialogue);
        }
    }

    public Dialogue ActivateDialogue() {
        foreach (Dialogue dialogue in dialogues) {
            if (dialogue.CheckReqs()) return dialogue;
        }

        return null;
    }
}
