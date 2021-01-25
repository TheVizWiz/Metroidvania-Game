using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[Serializable]
public class NPC {

    public string name;
    public string description;
    public string autoDeclineMessage;

    public List<Dialogue> dialogues;
    // Start is called before the first frame update


    public NPC() {
        dialogues = new List<Dialogue>();
    }

    public Dialogue ActivateDialogue() {
        foreach (Dialogue dialogue in dialogues) {
            if (dialogue.CheckReqs()) return dialogue;
        }

        return null;
    }

    public static NPC LoadNPCFromJSON(string filePath) {
        NPC npc =  JsonUtility.FromJson<NPC>((Resources.Load(filePath) as TextAsset).text);
        foreach (Dialogue dialogue in npc.dialogues) {
            dialogue.InitializeStaticLines();
        }
        return npc;
    }
}
