using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuestManager {

    private static string folder = "" + Path.DirectorySeparatorChar;
    public static Dictionary<string, Quest> quests;
    private static Dictionary<string, string> currentStepList;

    public static void Initialize() {
        
        string[] questPaths = SaveManager.ReadFileFromResources("QuestMaster");
        quests = new Dictionary<string, Quest>();
        foreach (string path in questPaths) {
            Quest q = Quest.LoadQuest(path);
            quests.Add(q.name, q);
        }
        
        currentStepList = SaveManager.LoadSaveObject<string>("quests")?.GetDictionary();
        
        foreach (KeyValuePair<string, Quest> quest in quests) {
            if (currentStepList != null && currentStepList.TryGetValue(quest.Key, out string currentStep)) {
                if (currentStep == "null") {
                    quests.Remove(quest.Key);
                }
                quest.Value.SetCurrentStep(currentStepList[currentStep]);
            } else {
                quest.Value.currentStep = quest.Value.startStep;
            }
        }
    }
    
    
    

    public static void UpdateQuests() {
        List<string> removeKeys = new List<string>();
        foreach (KeyValuePair<string, Quest> pair in quests) {
            pair.Value.CheckDone();
            if (pair.Value.currentStep == null) {
                removeKeys.Add(pair.Key);
                Debug.Log("finished Quest");
            }
        }

        foreach (string s in removeKeys) {
            quests.Remove(s);
        }
    }
}
