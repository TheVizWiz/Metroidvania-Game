using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraCollider : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera camera;

    void Start() {
        
    }


    void Update() {
        
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(GameManager.Constants.PLAYER_TAG)) {
            GameManager.cameraController.SetCurrentCam(camera);
        }
        
    }

    private void OnCollisionExit2D(Collision2D other) {
    }
}
