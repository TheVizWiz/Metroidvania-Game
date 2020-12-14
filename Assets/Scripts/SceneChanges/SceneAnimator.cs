using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAnimator : MonoBehaviour {

    private static readonly int exitHash = Animator.StringToHash("Exit");
    private static readonly int enterHash = Animator.StringToHash("Enter");
    
    
    private Animator animator;

    // Start is called before the first frame update
    void Awake() {
        GameManager.sceneAnimator = this;
        animator = GetComponent<Animator>();
        // EnterScene();
    }

    private void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public void EnterScene() {
        animator.SetTrigger(enterHash);
    }

    public void ExitScene() {
        animator.SetTrigger(exitHash);
    }
}
