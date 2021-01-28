using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using UnityEngine.UI;

public class SceneAnimator : MonoBehaviour {

    [SerializeField] private Image image;
    [SerializeField] public float time;
    
    
    private Animator animator;
    private bool isTweening;
    // Start is called before the first frame update
    void Awake() {
        GameManager.sceneAnimator = this;
        isTweening = false;
        // FadeIn();
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
        }).setOnComplete(() => isTweening = false).setEaseInOutSine();
    }

    public void ExitScene() {
        isTweening = true;
        LeanTween.value(image.gameObject, 0, 1, time).setOnUpdate((float val) => {
            Color color = image.color;
            color.a = val;
            ;
            image.color = color;
        }).setOnComplete(() => isTweening = false).setEaseInOutSine();
    }
    
    public void FadeIn(float time) {
        isTweening = true;
        LeanTween.value(image.gameObject, 1, 0, time).setOnUpdate((float val) => {
            Color color = image.color;
            color.a = val;
            ;
            image.color = color;
        }).setOnComplete(() => isTweening = false).setEaseInOutSine();
    }

    public void FadeOut(float time) {
        isTweening = true;
        LeanTween.value(image.gameObject, 0, 1, time).setOnUpdate((float val) => {
            Color color = image.color;
            color.a = val;
            ;
            image.color = color;
        }).setOnComplete(() => isTweening = false).setEaseInOutSine();
    }
}
