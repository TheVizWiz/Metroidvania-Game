using System;
using UnityEngine;

public class DamageBarrier : MonoBehaviour {

    [SerializeField] private GameObject player;

    private float distance;
    private Transform playerTransform;
    private Transform _transform;
    // Start is called before the first frame update
    private void Start() {
        playerTransform = player.transform;
        _transform = transform;
        distance = player.transform.position.y - _transform.position.y;
        SaveZoneHandler.barrier = this;
    }

    // CheckDone is called once per frame
    private void Update() {
        if (playerTransform.position.y - transform.position.y > distance) {
            ResetPosition();
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(Constants.PLAYER_TAG)) {
            SaveZoneHandler.RespawnAtLastZone(other.gameObject);
        }
    }

    public void ResetPosition() {
        _transform.position = new Vector3(_transform.position.x, playerTransform.position.y - distance,
            _transform.position.z);
    }
}
