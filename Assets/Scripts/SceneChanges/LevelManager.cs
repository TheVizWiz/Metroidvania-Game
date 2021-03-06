﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour {
    
    
    [HideInInspector] public int areaIndex;
    public Vector2 playerSpawn;
    public EnterExitArea[] areas;



    void Awake() {
        if (!GameManager.isInitialized) {
            Inventory.Initialize();
            QuestManager.Initialize();
            SaveManager.Initialize();
            NPCManager.Initialize();
            GameManager.isInitialized = true;
            PlayerPrefs.DeleteAll();
            Inventory.PickUp("mayastepstarter", 1);
        }
        areaIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start() {
        foreach (EnterExitArea area in areas) {
            area.Setup();
        }
        GameManager.EnterScene(this);
    }


    void Update() {
    }
}
