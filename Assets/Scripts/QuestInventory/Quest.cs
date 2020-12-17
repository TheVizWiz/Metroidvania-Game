using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Animations;
using UnityEngine;
using Debug = UnityEngine.Debug;

[Serializable]
public class Quest {
    
    public string name;
    public string description;
    public QuestStep currentStep, startStep;
    private Dictionary<string, QuestStep> steps;

    static Quest() {
    }

    public Quest(QuestStep step) {
        startStep = step;
        currentStep = startStep;
        steps = new Dictionary<string, QuestStep>();
    }

    public Quest(string name) {
        this.name = name;
        steps = new Dictionary<string, QuestStep>();
    }

    public void Update() {
        currentStep = currentStep.CheckTransition();
        if (currentStep == null) EndQuest(); 
    }

    public void EndQuest() {
        
    }

    public static Quest LoadQuest(string path) {
        string[] text = SaveManager.ReadFileFromResources(path);
        if (text == null) return null;
        Quest quest = new Quest(text[0]);
        quest.description = text[1];


        for (int i = 3; i < text.Length;) {
            
            //setup new quest step
            QuestStep step = new QuestStep();
            step.name = text[i];
            i++;
            step.description = text[i];

            //find number of transitions
            i++;
            int numTransitions = int.Parse(text[i]);
            int transitionIndex = 0;
            if (numTransitions > 0) {
                i++;
                transitionIndex = i + numTransitions;
            }

            //iterate through all the transitions
            for (int j = 0; j < numTransitions; j++) {
                
                //setup new QuestTransition
                string[] transitionLine = text[i].Split(new[]{' '}, 2);
                QuestTransition transition = new QuestTransition();

                if (transitionLine.Length == 2) {
                    transition.endQuest = quest.steps[transitionLine[1]];
                }
                else transition.endQuest = null;
                
                //find number of reqs in given transition
                int numRequirements = int.Parse(transitionLine[0]);
                int reqIndex = 0;
                if (numRequirements > 0) {
                    i++;
                    reqIndex = i + numRequirements;
                }

                //iterate through all the reqs for the given transition
                for (; i < reqIndex; i++) {
                    string[] reqs = text[i].Split(new[] {' '}, 2);
                    transition.requirements.Add(reqs[1], int.Parse(reqs[0]));
                }
                
                step.transitions.Add(transition);

            }

            quest.steps.Add(step.name, step);
        }

        quest.startStep = quest.steps[text[2]];

        return quest;
    }


    public override string ToString() {
        string s = "";
        s += name + "\t" + description + "\n";
        s += startStep.ToString();
        return s;
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

    public override string ToString() {
        string s = name + "\t" + description;
        Debug.Log(transitions.Count);
        if (transitions.Count == 0) return s;
        foreach (QuestTransition transition in transitions) {
            s += "\n\t" + transition.ToString();
        }

        return s;
    }
}

[Serializable]
public class QuestTransition {
    public QuestStep endQuest;
    public Dictionary<string, int> requirements;


    public QuestTransition() {
        requirements = new Dictionary<string, int>();
    }

    public bool CheckReqs() {
        foreach (KeyValuePair<string, int> req in requirements) {
            if (!Inventory.HasValue(req.Key, req.Value)) {
                return false;
            }
        }

        return true;
    }

    public override string ToString() {
        string s = "";
        foreach (KeyValuePair<string, int> keyValuePair in requirements) {
            s += keyValuePair.Value + "\t" + keyValuePair.Key + "\n";
        }

        return s + ((endQuest == null) ? "" : endQuest.ToString());
    }
}
