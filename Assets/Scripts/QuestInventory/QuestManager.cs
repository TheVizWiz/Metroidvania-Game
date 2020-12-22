using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuestManager {

    private static string folder = "" + Path.DirectorySeparatorChar;
    public static Dictionary<string, Quest> quests;

    public static void Initialize() {
        string[] questPaths = SaveManager.ReadFileFromResources("QuestMaster");
        Debug.Log(questPaths.Length);
        quests = new Dictionary<string, Quest>();
        foreach (string path in questPaths) {
            Quest q = Quest.LoadQuest(path);
            quests.Add(q.name, q);
        }
        
        Dictionary<string, string> stepList = SaveManager.LoadSaveObject<string>("quests")?.GetDictionary();
        if (stepList == null) {
            Debug.Log("null step list");
        } else {

            foreach (KeyValuePair<string, Quest> quest in quests) {
                if (stepList.TryGetValue(quest.Key, out string currentStep)) {
                    quest.Value.SetCurrentStep(stepList[currentStep]);
                }
            }
        }

        foreach (KeyValuePair<string, Quest> quest in quests) {
            Debug.Log(quest.ToString());
        }
        // Inventory.PickUp(new InventoryItem("thought", 1));

    }
    
    
    

    public static void UpdateQuests() {
        foreach (KeyValuePair<string, Quest> q in quests) {
            q.Value.CheckDone();
        }
    }
}
