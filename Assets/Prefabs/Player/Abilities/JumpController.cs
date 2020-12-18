using UnityEngine;
using UnityEngine.Serialization;

public class JumpController : AbilityController {

    private bool hasJumped;

    [SerializeField] private float jumpVelocity;
    [SerializeField] private float maxTime;

    public override bool Activate() {
        if (!hasJumped && !movement.isInAir && Input.GetButtonDown("Jump")) {
            hasJumped = true;
            isActive = true;
            elapsedTime = 0;
            animator.SetBool(PlayerMovement.jumpString, true);
            return true;

        }

        return false;
    }


    public override bool Stop() {
        isActive = false;
        hasJumped = false;
        body.velocity = Vector2.zero;
        animator.SetBool(PlayerMovement.jumpString, false);
        return true;
    }

    // Start is called before the first frame update
    private void Start() {
        Setup();
        animString = "Jump";
    }

    // CheckDone is called once per frame

    private void Update() {
        if (!movement.canMove) {
            Stop();
        }
        if (isActive) {
            if (Input.GetButton("Jump") && elapsedTime < maxTime) {
                elapsedTime += Time.deltaTime;
                // body.AddForce(Physics2D.gravity * (-1 * body.gravityScale));//
                body.velocity = Vector2.up * jumpVelocity;
            }
            else if (Input.GetButtonUp("Jump")) {
                Stop();
            }

            if (elapsedTime >= maxTime) {
                Stop();
                body.velocity = Vector2.up * jumpVelocity;
            }
        }


    }

    public override void OnPlayerCollisionEnter2D(Collision2D collision) {
        if (isActive && collision.gameObject.layer == Constants.STANDABLE_LAYER) {
            hasJumped = false;
            Stop();
            elapsedTime = 0;
        }
    }


}