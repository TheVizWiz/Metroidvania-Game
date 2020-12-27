using System;
using UnityEditor;
using UnityEngine;

public class ShieldController : AbilityController {


    private static readonly int startFlag = Animator.StringToHash("Start");
    private static readonly int endFlag = Animator.StringToHash("End");


    [Header("Shields")]
    [SerializeField] private GameObject shieldHorizontal;
    [SerializeField] private GameObject shieldTop;
    [SerializeField] private GameObject shieldBottom;

    [Header("Aegis Eclipse")]
    [SerializeField] private float eclipseRegenRate;

    [Header("Elara's Strength")]
    [SerializeField] private float strengthBashDownTime;
    [SerializeField] private float strengthBashDamage;
    [SerializeField] private float strengthBashTime;
    [SerializeField] private float strengthBashDistance;




    private Vector2 bashPosition;
    private Animator hAnimator, tAnimator, bAnimator;

    [HideInInspector] public bool canPogo;
    private int direction; // 3 directions -> 1 = top, 2 = horizontal (right/left), 3 = bottom
    private bool isBashing;
    private float timeSinceLastBash;



    // Start is called before the first frame updatei
    private void Start() {
        Setup();
        fileString = "shieldController";
        canPogo = true;
        isBashing = false;
        timeSinceLastBash = 0;
        hAnimator = shieldHorizontal.GetComponent<Animator>();
        tAnimator = shieldTop.GetComponent<Animator>();
        bAnimator = shieldBottom.GetComponent<Animator>();
    }

    // CheckDone is called once per frame
    private void Update() {
        if (isActive) {
            if (movement.isInUI) {
                Stop();
            }
            if (Input.GetButtonUp("Shield")) {
                switch (direction) {
                    case 1:
                        tAnimator.SetTrigger(endFlag);
                        break;

                    case 2:
                        hAnimator.SetTrigger(endFlag);
                        break;

                    case 3:
                        bAnimator.SetTrigger(endFlag);
                        break;

                }

                Stop();
            }

            if (isBashing) {
                if (direction != 3) {
                    bashPosition += Vector2.right *
                                    (movement.lookDirection * strengthBashDistance / strengthBashTime * Time.deltaTime);
                    body.MovePosition(bashPosition);
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime >= strengthBashTime) {
                        isBashing = false;
                        movement.canMove = true;
                        timeSinceLastBash = 0;
                    }
                }
            } else {
                timeSinceLastBash += Time.deltaTime;
            }
        }
    }

    public override bool Activate() {
        if (isBashing) return true;
        float horizontal = Mathf.Abs(Input.GetAxis("Horizontal")); //getting stick/movement/look input
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(vertical) > horizontal) {
            if (vertical < -Constants.INPUT_ERROR) direction = 3;
            else if (vertical > Constants.INPUT_ERROR) direction = 1;
        } else if (horizontal > Constants.INPUT_ERROR) {
            direction = 2;
        } else direction = 2; //classified input based on which is largest - that will be shielded direction.

        //checking upgrade level and activating accordingly.
        if (!isActive) {
            switch (direction) {
                case 1: //top
                    if (upgradeLevel >= 3 && movement.canMove && Input.GetButtonDown("Shield")) {
                        tAnimator.SetTrigger(startFlag);
                        isActive = true;
                    }
                    break;

                case 2: //horizontal
                    if (upgradeLevel >= 1 && movement.canMove && !movement.isInAir && Input.GetButtonDown("Shield")) {
                        hAnimator.SetTrigger(startFlag);
                        isActive = true;
                        movement.canTurn = false;
                        if (!(upgradeLevel > 2)) movement.canMove = false;
                    }
                    break;

                case 3:
                    if (canPogo && movement.isInAir && movement.canMove && upgradeLevel >= 3 && Input.GetButtonDown("Shield")) {
                        isActive = true;
                        bAnimator.SetTrigger(startFlag);
                            movement.canTurn = true;

                    }
                    break;
            }

        }

        if (!isBashing && movement.canAttack && Input.GetAxis("Strike") > Constants.AXIS_SENSE &&
            upgradeLevel >= 2 && timeSinceLastBash > strengthBashDownTime && isActive
            && direction == 2 && !movement.isInAir) {
            isBashing = true;
            bashPosition = body.position;
            movement.canMove = false;
            elapsedTime = 0;
            return true;
        } else if (!isBashing && movement.canAttack && movement.canBashDownwards &&
                   Input.GetAxis("Strike") > Constants.AXIS_SENSE &&
                   upgradeLevel >= 2 && timeSinceLastBash > strengthBashDownTime && isActive
                   && direction == 3 && movement.isInAir) {
            isBashing = true;
            bashPosition = body.position;
            movement.canMove = false;
            elapsedTime = 0;
            return true;
        }

        return isActive;
    }

    public override bool Stop() {
        if (direction == 1) {
            tAnimator.SetTrigger(endFlag);
        } else if (direction == 2) {
            hAnimator.SetTrigger(endFlag);
        } else if (direction == 3) {
            bAnimator.SetTrigger(endFlag);
        }
        movement.canMove = true;
        movement.canTurn = true;
        isActive = false;
        return true;
    }

    public void OnShieldCollision(Collision2D other) {
        if (other.gameObject.layer == Constants.ENEMY_LAYER) {

        }

    }
}
