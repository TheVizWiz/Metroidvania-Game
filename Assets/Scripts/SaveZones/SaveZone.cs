using System;
using UnityEngine;

public class SaveZone : MonoBehaviour {



    // Start is called before the first frame update
    private void Start() {

    }

    // CheckDone is called once per frame
    private void Update() {

    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(Constants.PLAYER_TAG)) {
            SaveZoneHandler.SetSafeZone(this);
        }
    }
}
