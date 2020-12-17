using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour {
    
    
    public int areaIndex;
    public Vector2 playerSpawn;
    public EnterExitArea[] areas;

    private int currentCameraIndex;

    

    void Awake() {
        Quest q = Quest.LoadQuest("tester");
        Debug.Log(q.ToString());
    }

    private void Start() {
        GameManager.EnterScene(this);
    }


    void Update() {
        
    }
}
