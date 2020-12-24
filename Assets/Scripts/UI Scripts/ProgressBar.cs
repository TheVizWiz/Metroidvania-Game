using System;
using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class ProgressBar : MonoBehaviour, IProgressBar {
    
    [SerializeField] private float moveTime;
    [SerializeField] private Image mainBar;
    [SerializeField] private float min;
    [SerializeField] private float max;
    [SerializeField] public float current;
    
    [HideInInspector] public bool hasWaitTime = true;
    [HideInInspector] public float waitTime;
    [HideInInspector] public Image instantBar;

    private float timeWaited, timeMoved;
    private bool isChanging;
    private float tempVal;


    // Start is called before the first frame update
    void Start() {
        if (!hasWaitTime && instantBar != null) {
            instantBar.gameObject.SetActive(false);
        }

        if (instantBar != null) {
            instantBar.fillAmount = (current - min) / (max - min);
        }

        mainBar.fillAmount = (current - min) / (max - min);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            SetCurrentValue(GetCurrentValue() - 10);
        }
    }

    public void SetCurrentValue(float newVal) {
        if (newVal > max || newVal < min) {
            print("new value out of bounds");
            return;
        }
        if (isChanging) {
            if (hasWaitTime) {
                timeWaited = 0;
                current = newVal;
            }

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
            
            if (hasWaitTime && timeWaited < waitTime) {
                instantBar.fillAmount = (current - min) / (max - min);
                timeWaited += Time.deltaTime;
                yield return null;
            } else if (timeMoved <= moveTime) {
                timeMoved += Time.deltaTime;
                float val = 0;
                if (!FloatComparer.AreEqual(current, newVal, Single.Epsilon)) {
                    newVal = current;
                    tempVal = Mathf.SmoothStep(oldCurrent, current, timeMoved / moveTime);
                    val = tempVal;
                } else {
                    oldCurrent = Mathf.SmoothStep(tempVal, current, timeMoved / moveTime);
                    val = oldCurrent;
                }
                mainBar.fillAmount = (val - min) / (max - min);
                yield return null;
            } else {
                break;
            }

        }
        isChanging = false;


    }


}



#if UNITY_EDITOR
[CustomEditor(typeof(ProgressBar))]
public class HorizontalBarCustomEditor : Editor {

    private readonly string[] directions = {"Trail On Decrease", "Trail On Increase"};
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        ProgressBar bar = (ProgressBar) target;
        
        bar.hasWaitTime = EditorGUILayout.Toggle("Has Wait Time", bar.hasWaitTime); 
        if (bar.hasWaitTime) {
            EditorGUI.indentLevel++;
            bar.waitTime = EditorGUILayout.FloatField("Wait Time", bar.waitTime);
            bar.instantBar = EditorGUILayout.ObjectField("Instant Moving Bar", bar.instantBar, typeof(Image), true) as Image;
            EditorGUI.indentLevel--;
        }

    }
}
#endif
