﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using UnityEngine.UI;

public class SceneAnimator : MonoBehaviour {

    [SerializeField] private Image image;
    [SerializeField] private float time;
    
    
    private Animator animator;
    private bool isTweening;
    // Start is called before the first frame update
    void Awake() {
        GameManager.sceneAnimator = this;
        isTweening = false;
        // EnterScene();
    }

    private void StopTween() {
        isTweening = false;
    }

    public bool IsTweening() {
        return isTweening;
    }

    public void EnterScene() {
        isTweening = true;
        LeanTween.value(image.gameObject, 1, 0, time).setOnUpdate((float val) => {
            Color color = image.color;
            color.a = val;
            ;
            image.color = color;
        }).setOnComplete(StopTween).setEaseInOutSine();
    }

    public void ExitScene() {
        isTweening = true;
        LeanTween.value(image.gameObject, 0, 1, time).setOnUpdate((float val) => {
            Color color = image.color;
            color.a = val;
            ;
            image.color = color;
        }).setOnComplete(StopTween).setEaseInOutSine();
    }
}
