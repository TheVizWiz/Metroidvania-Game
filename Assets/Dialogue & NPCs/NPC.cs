using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NPC{

    public string name;
    public string description;
    public string autoDeclineMessage;

    private List<Dialogue> dialogues;
    // Start is called before the first frame update


    public NPC() {
        dialogues = new List<Dialogue>();
    }
    public static NPC LoadNPC(string filePath) {
        string[] file = SaveManager.ReadFileFromResources(filePath);
        NPC npc = new NPC();
        int currentLine = 0;
        npc.name = file[currentLine];
        npc.description = file[++currentLine];
        npc.autoDeclineMessage = file[++currentLine];
        ++currentLine;
        while (currentLine < file.Length) {
            Dialogue dialogue = new Dialogue();
            dialogue.LoadDialogue(file, ref currentLine);
            npc.dialogues.Add(dialogue);
        }

        return npc;
    }

    public Dialogue ActivateDialogue() {
        foreach (Dialogue dialogue in dialogues) {
            if (dialogue.CheckReqs()) return dialogue;
        }

        return null;
    }
}
