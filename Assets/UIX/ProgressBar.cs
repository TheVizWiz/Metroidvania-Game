using System;
using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class ProgressBar : MonoBehaviour, IProgressBar {
    
    [SerializeField] private float moveTime;
    [FormerlySerializedAs("mainBar")] [SerializeField] private Image backBar;
    [SerializeField] public float min;
    [SerializeField] public float max;
    [SerializeField] public float current;
    
    [HideInInspector] public bool hasWaitTime;
    [HideInInspector] public float waitTime;
    [HideInInspector] public Image frontBar;
    [HideInInspector] public ProgressBarType type;

    private float timeWaited, timeMoved;
    private bool isChanging, initializedWait;
    private float tempVal;


    // Start is called before the first frame update
    void Start() {
        if (!hasWaitTime) {
            print("no wait time");
        }
        if (!hasWaitTime && frontBar != null) {
            print("no wait time");
            frontBar.gameObject.SetActive(false);
        }

        if (frontBar != null) {
            frontBar.fillAmount = (current - min) / (max - min);
        }

        backBar.fillAmount = (current - min) / (max - min);
    }

    // Update is called once per frame
    void Update() {
    }

    public void SetMax(float max) {
        this.max = max;
    }

    public void SetMin(float min) {
        this.min = min;
    }

    public void SetCurrentValue(float newVal, bool checkBounds) {
        if (checkBounds && (newVal > max || newVal < min)) {
            print("new value out of bounds");
            return;
        }
        if (isChanging) {
            if (hasWaitTime) {
                timeWaited = 0;
                initializedWait = false;
            }
            current = newVal;
            timeMoved = 0;
        } else {
            StartCoroutine(ChangeValues(newVal));
        }
    }

    public float GetCurrentValue() {
        return current;
    }

    public IEnumerator ChangeValues(float newVal) {
        //current: always the current absolute value
        //temp: always the current changing value
        //newVal: what the current should be set to.
        
        tempVal = current;
        float oldCurrent = current;
        current = newVal;
        timeMoved = 0;
        timeWaited = 0;
        isChanging = true;

        while (true) {

            if (!initializedWait && hasWaitTime) {
                initializedWait = true;
                if (type == ProgressBarType.TrailOnDecrease) {
                    if (tempVal > newVal) {
                        frontBar.fillAmount = (current - min) / (max - min);
                    } else {
                        timeWaited = waitTime + 1;
                    }
                } else if (type == ProgressBarType.TrailOnIncrease) {
                    if (tempVal < newVal)
                        backBar.fillAmount = (current - min) / (max - min);
                    else timeWaited = timeWaited + 1;
                }
            }
            
            if (hasWaitTime && timeWaited < waitTime) {
                timeWaited += Time.deltaTime;
                yield return null;
            } else if (timeMoved <= moveTime) {
                timeMoved += Time.deltaTime;
                float val = 0;
                if (!FloatComparer.AreEqual(current, newVal, float.Epsilon)) {
                    newVal = current;
                    tempVal = oldCurrent;
                    val = tempVal;
                } else {
                    oldCurrent = Mathf.SmoothStep(tempVal, current, timeMoved / moveTime);
                    val = oldCurrent;
                }

                if (type == ProgressBarType.TrailOnDecrease) {
                    backBar.fillAmount = (val - min) / (max - min);
                    if (tempVal < newVal) {
                        frontBar.fillAmount = (val - min) / (max - min);
                    }
                } else if (type == ProgressBarType.TrailOnIncrease) {
                    frontBar.fillAmount = (val - min) / (max - min);
                    if (tempVal > newVal) {
                        backBar.fillAmount = (val - min) / (max - min);
                    }
                }

                yield return null;
            } else {
                break;
            }

        }
        isChanging = false;
        initializedWait = false;


    }
    
    
    #if UNITY_EDITOR
    public void UpdateBarFromEditor(float min, float max, float current) {
        this.current = current;
        this.min = min;
        this.max = max;
        if (frontBar != null) {
            frontBar.fillAmount = (current - min) / (max - min);
        }
        if (backBar != null) {
            backBar.fillAmount = (current - min) / (max - min);
        }
    }
    #endif


}



#if UNITY_EDITOR
[CustomEditor(typeof(ProgressBar)), CanEditMultipleObjects]
public class HorizontalBarCustomEditor : Editor {

    private SerializedProperty waitTime, hasWaitTime, instantBar, type;

    // private readonly string[] directions = {"Trail On Decrease", "Trail On Increase"};

    private void OnEnable() {
        waitTime = serializedObject.FindProperty("waitTime");
        hasWaitTime = serializedObject.FindProperty("hasWaitTime");
        instantBar = serializedObject.FindProperty("frontBar");
        type = serializedObject.FindProperty("type");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(hasWaitTime, new GUIContent("Has Wait Time"));
        if (hasWaitTime.boolValue) {
            EditorGUILayout.PropertyField(type, new GUIContent("Trail Type"));
            EditorGUILayout.PropertyField(waitTime, new GUIContent("Wait Time"));
            EditorGUILayout.PropertyField(instantBar, new GUIContent("Front Bar"));
        }
        // ;
        // EditorGUILayout.Toggle()
        // bar.hasWaitTime = EditorGUILayout.Toggle("Has Wait Time", bar.hasWaitTime); 
        // if (bar.hasWaitTime) {
        //     EditorGUI.indentLevel++;1
        //     bar.waitTime = EditorGUILayout.FloatField("Wait Time", bar.waitTime);
        //     bar.instantBar = EditorGUILayout.ObjectField("Instant Moving Bar", bar.instantBar, typeof(Image), true) as Image;
        //     EditorGUI.indentLevel--;
        // }

        serializedObject.ApplyModifiedProperties();

    }
}
#endif


public enum ProgressBarType {
    TrailOnDecrease,
    TrailOnIncrease,
}