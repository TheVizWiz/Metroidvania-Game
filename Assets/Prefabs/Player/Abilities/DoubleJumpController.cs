using UnityEngine;
using UnityEngine.Serialization;

public class DoubleJumpController : AbilityController {

    private bool hasJumped;

    [SerializeField] private float jumpVelocity;
    [SerializeField] private float maxTime;
    // Start is called before the first frame update

    private void Start() {
        Setup();
        animString = "DoubleJump";
    }

    // CheckDone is called once per frame
    private void Update() {
        if (!movement.canMove) {
            Stop();
        }
        if (isActive) {
            if (Input.GetButton("Jump") && elapsedTime < maxTime) {
                elapsedTime += Time.deltaTime;
                // body.AddForce(Physics2D.gravity * (-1 * body.gravityScale));
                body.velocity = Vector2.up * jumpVelocity;
            }

            else if (Input.GetButtonUp("Jump")) {
                Stop();
            }

            if (elapsedTime >= maxTime) {
                isActive = false;
                elapsedTime = 0;
            }
        }


    }

    public override bool Activate() {
        if (!hasJumped && movement.isInAir && upgradeLevel > 0 && Input.GetButtonDown("Jump")) {
            hasJumped = true;
            isActive = true;
            elapsedTime = 0;
            animator.SetBool(PlayerMovement.doubleJumpString, true);
            // body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            return true;
        }
        return false;
    }

    public override bool Stop() {
        isActive = false;
        body.velocity = Vector2.zero;
        animator.SetBool(PlayerMovement.doubleJumpString, false);
        return true;
    }

    public override void OnPlayerCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == Constants.STANDABLE_LAYER) {
            hasJumped = false;
            Stop();
        }
    }
}