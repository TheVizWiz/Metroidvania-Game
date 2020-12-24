using System;
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
            QuestManager.Initialize();
            Inventory.Initialize();
            GameManager.isInitialized = true;
        }
        areaIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start() {
        GameManager.EnterScene(this);
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Inventory.PickUp("steps", 20);
        }
    }
}
