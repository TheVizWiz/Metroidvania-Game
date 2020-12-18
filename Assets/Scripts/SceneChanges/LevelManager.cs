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
        SaveManager.Initialize();
        Inventory.Initialize();
        QuestManager.Initialize();
    }

    private void Start() {
        GameManager.EnterScene(this);
    }


    void Update() {
        
    }
}
