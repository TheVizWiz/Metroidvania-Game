using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NPCManager {
    private static List<NPC> npcs;

    public static void Initialize() {
        string[] npclist = SaveManager.ReadFileFromResources(Path.Combine("NPCs", "master"));
        foreach (string s in npclist) {
            Debug.Log(s);
        }
    }
}
