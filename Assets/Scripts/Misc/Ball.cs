using System;
using System.Collections;
using Interfaces;
using UnityEngine;

public class Ball : MonoBehaviour, ICarryable {

    private Transform parentTransform;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    
    private bool isCarried;
    protected bool used;
    private float timeWaited;
    private Vector3 offset;
    
    
    public float timeToWait;
    public float releaseForce;

    // Start is called before the first frame update
    public void Start() {
        isCarried = false;
        timeWaited = 0;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        used = false;
    }

    // CheckDone is called once per frame
    public void Update() {
        UpdatePosition();

        if (!isCarried && timeWaited <= timeToWait) {
            timeWaited += Time.deltaTime;
        }

        if (timeWaited >= timeToWait && !collider.enabled && !used) {
            collider.enabled = true;
        }
    }

    public void UpdatePosition() {
        if (isCarried) {
            collider.enabled = false;
            rigidbody.Sleep();
            transform.position = parentTransform.position + offset;
        } else {
        }
    }

    public void Pickup(Transform carrierTransform, Vector3 offset) {
        isCarried = true;
        parentTransform = carrierTransform;
        this.offset = offset;
        collider.enabled = false;
        rigidbody.Sleep();
        timeWaited = 0;

    }

    public void Release() {
        isCarried = false;
        collider.enabled = true;
        rigidbody.WakeUp();
        rigidbody.AddForce(Vector2.up * releaseForce, ForceMode2D.Impulse);
    }

    private void OnCollisionStay2D(Collision2D other) {    
        if (collider.gameObject.layer == GameManager.Constants.PLAYER_LAYER) {
            Physics2D.IgnoreCollision(collider, other.collider);
        }
    }
}