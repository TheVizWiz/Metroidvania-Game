using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CavesFinalBoss;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA;

public static class GameManager {
    
    public static GameObject player;
    public static SceneAnimator sceneAnimator;
    public static LevelManager levelManager;
    public static CameraController cameraController;
    public static DialogueManager dialogueManager;

    public static LayerMask enemyLayerMask;
    public static LayerMask strikableLayerMask;
    public static LayerMask slashableLayerMask;

    private static int oldIndex;
    private static bool isLoading;



    public static IEnumerator LoadScene(int newIndex) {
        if (isLoading) yield break;
        else isLoading = true;

        float time = 0;
        oldIndex = levelManager.areaIndex;
        sceneAnimator.ExitScene();
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(newIndex);
        operation.allowSceneActivation = false;


        while (operation.progress < 0.9 && time < 0.5) {
            time += Time.deltaTime;
                yield return null;
        }
        
        // Debug.Log("finished load");
        operation.allowSceneActivation = true;

    }
    
    public static void EnterScene(LevelManager levelManager) {
        GameManager.levelManager = levelManager;
        foreach (EnterExitArea area in levelManager.areas) {
            if (area.nextSceneIndex == oldIndex) {
                levelManager.StartCoroutine(area.Enter());
                break;
            }
        }
        sceneAnimator.EnterScene();
        isLoading = false;
    }
    
    
}