using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager {

    public static List<Quest> quests;

    static QuestManager() {
        quests = new List<Quest>();
    }

    public static void UpdateQuests() {
        foreach (Quest q in quests) {
            q.Update();
        }
    }
}
