using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public class Quest {
    
    public string name;
    public QuestStep currentStep, startStep;

    public Quest(QuestStep step) {
        startStep = step;
        currentStep = startStep;
    }

    public void Update() {
        currentStep = currentStep.CheckTransition();
    }
    
}

[Serializable]
public class QuestStep {

    public string name;
    public string description;
    public List<QuestTransition> transitions;

    public QuestStep() {
        transitions = new List<QuestTransition>();
    }

    public QuestStep(string name) {
        this.name = name;
        this.transitions = new List<QuestTransition>();
    }

    public QuestStep(string name, string description) {
        this.name = name;
        this.description = description;
        transitions = new List<QuestTransition>();
    }

    public QuestStep CheckTransition() {
        foreach (QuestTransition transition in transitions) {
            if (transition.CheckReqs()) return transition.endQuest;
        }

        return this;
    }
}

[Serializable]
public class QuestTransition {
    public QuestStep endQuest;
    public Dictionary<string, int> requirements;

    public bool CheckReqs() {
        foreach (KeyValuePair<string, int> req in requirements) {
            if (!Inventory.HasValue(req.Key, req.Value)) {
                return false;
            } 
        }

        return true;
    }
}
