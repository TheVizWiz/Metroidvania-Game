using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovingPlatformPlayerController : MonoBehaviour {

    private Transform transform;

    private void Start() {
        transform = gameObject.transform;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.gameObject.layer == GameManager.Constants.PLAYER_LAYER)
            GameManager.playerMovement.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.collider.gameObject.layer == GameManager.Constants.PLAYER_LAYER) {
            GameManager.playerMovement.transform.parent = null;
            DontDestroyOnLoad(GameManager.player);
        }
    }
}
