using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerZone : MonoBehaviour {

    private new Collider2D collider;
    [SerializeField] private Scene nextScene;

    private void Awake() {
        collider = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    private void Start() {
    }

    // Update is called once per frame
    private void Update() {
    }
}