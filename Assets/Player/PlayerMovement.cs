using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;
using Interfaces;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ICarrier {
    public static int
        LOOK_DIRECTION_LEFT = -1, LOOK_DIRECTION_RIGHT = 1; // -1 is left, 1 is right, 0 is nothing - 0 is edge case.

    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D topCollider;
    [SerializeField] private Collider2D sideCollider;
    [SerializeField] private Collider2D bottomCollider;
    [SerializeField] private Vector3 carryPosition;
    [SerializeField] private float carryCooldown;
    [SerializeField] private float moveSpeed;

    public int damageLevel;
    public List<AbilityController> abilities;

    private Rigidbody2D body;
    [HideInInspector] public Dictionary<string, int> upgradeLevels;
    [HideInInspector] public PlayerInput input;
    [HideInInspector] public PlayerMain main;
    [HideInInspector] public bool canAttack;
    [HideInInspector] public bool canMove;
    [HideInInspector] public bool canTurn;
    [HideInInspector] public bool canInteract;
    [HideInInspector] public bool isInAir;
    [HideInInspector] public bool isCarrying;
    [HideInInspector] public bool canBashDownwards;
    [HideInInspector] public IInteractable interactable;
    [HideInInspector] public int lookDirection = 1;
    private int horizontalInput;

    private float timeSinceLastCarry;
    private ICarryable carryable;
    private Vector3 lastSafePosition;
    private float hInput;

    public static readonly string walkString = "Walk";
    public static readonly string fallString = "Fall";
    public static readonly string jumpString = "Jump";
    public static readonly string diveString = "Dive";
    public static readonly string chargeString = "Charge";
    public static readonly string doubleJumpString = "DoubleJump";
    public static readonly string ascendantString = "SaveSpot";
    public static readonly string filePath = "abfpth";


    // Start is called before the first frame update
    private void Awake() {
        isInAir = false;
        canBashDownwards = false;
        body = GetComponent<Rigidbody2D>();
        main = GetComponent<PlayerMain>();
        canMove = true;
        canTurn = true;
        canAttack = true;
        input = new PlayerInput();
        input.Enable();
        if (GameManager.playerMovement == null) {
            GameManager.playerMain = main;
            GameManager.playerMovement = this;
            GameManager.player = this.gameObject;
        } else {
            Destroy(this.gameObject);
            return;
        }

        input.Player.Move.performed += context => { hInput = context.action.ReadValue<Vector2>().x; };
        input.Player.Move.canceled += context => { hInput = 0; };
        input.Player.Strike.started += context => {
            if (isCarrying) DropCarryable();
        };
        input.General.Interact.started += context => {
            if (canInteract && !isInAir) {
                interactable?.Interact();
                // input.Player.Disable();
            }
        };

        DontDestroyOnLoad(gameObject);
        // LoadLevels();
        upgradeLevels = new Dictionary<string, int>();
    }

    // CheckDone is called once per frame
    private void Update() {

        if (canMove) {
            if (hInput > GameManager.Constants.INPUT_ERROR) {
                if (horizontalInput != 1 && !isInAir) {
                    animator.SetBool(walkString, true);
                }

                horizontalInput = 1;
            } else if (hInput < -GameManager.Constants.INPUT_ERROR) {
                if (horizontalInput != -1 && !isInAir) {
                    animator.SetBool(walkString, false);
                }

                horizontalInput = -1;
            } else {
                horizontalInput = 0;
            }


            if (horizontalInput == 0 && animator.GetBool(walkString)) {
                animator.SetBool(walkString, false);
            } else if (horizontalInput != 0 && !animator.GetBool(walkString) && !isInAir) {
                animator.SetBool(walkString, true);
            }

        }

        if (isInAir && !animator.GetBool(fallString)) {
            animator.SetBool(fallString, true);
            animator.SetTrigger("FallTrigger");
        }

        if (canTurn) {
            SetLookDirection(horizontalInput);
            if (horizontalInput != 0) lookDirection = horizontalInput;
        }


        bool activated = false;
        foreach (AbilityController controller in abilities) {
            if (!activated) activated = controller.Activate();
            else controller.Stop();
        }

        if (isCarrying && carryable == null) {
            isCarrying = false;
        }

        if (!isCarrying) {
            timeSinceLastCarry += Time.deltaTime;

            if (timeSinceLastCarry >= carryCooldown) {
                // Physics2D.IgnoreLayerCollision(GameManager.Constants.PLAYER_LAYER, GameManager.Constants.CARRYABLE_LAYER, false);
            }
        }
    }

    private void FixedUpdate() {
        if (canMove) 
            body.velocity = new Vector2((horizontalInput * moveSpeed), body.velocity.y);

    }

    public void SetLookDirection(int direction) {
        Transform transform1 = transform;
        Vector3 localScale = transform1.localScale;
        if (direction == LOOK_DIRECTION_LEFT) {
            if (transform1.localScale.x > 0)
                transform.localScale = new Vector3(transform1.localScale.x * -1, localScale.y,
                    localScale.z);
        } else if (direction == LOOK_DIRECTION_RIGHT)
            if (localScale.x < 0)
                transform.localScale = new Vector3(transform1.localScale.x * -1, localScale.y,
                    localScale.z);
    }

    public AbilityController GetActiveAbility() {
        foreach (AbilityController ability in abilities)
            if (ability.IsActive())
                return ability;
        return null;
    }

    public Transform GetCarrierTransform() {
        return transform;
    }

    public void DropCarryable() {
        carryable.Release();
        isCarrying = false;
        canAttack = true;
        timeSinceLastCarry = 0;
    }

    public void PickupCarryable(Collision2D collision) {
        ICarryable carryable = collision.collider.GetComponent<Ball>();
        if (carryable != null) {
            if (!isCarrying && timeSinceLastCarry >= carryCooldown &&
                collision.collider.CompareTag(GameManager.Constants.CARRYABLE_TAG)) {
                GameManager.Constants.LayerToLayerMask(1);
                carryable.Pickup(transform, carryPosition);
                isCarrying = true;
                canAttack = false;
                this.carryable = carryable;
                // Physics2D.IgnoreLayerCollision(GameManager.Constants.PLAYER_LAYER, GameManager.Constants.CARRYABLE_LAYER);
            }
        }
    }

    public void SetAnimationBool(string s, bool b) => animator.SetBool(s, b);
    public void SetAnimationFloat(string s, float f) => animator.SetFloat(s, f);
    public void SetAnimationTrigger(string s) => animator.SetTrigger(s);
    public void SetAnimationInt(string s, int i) => animator.SetInteger(s, i);

    /// <summary>
    /// Sets mobility of character based on flag
    /// modifies canMove, canTurn, canAttack;
    /// </summary>
    /// <param name="flag">flag of what to set everything to</param>
    public void SetMobility(bool flag) {
        canTurn = canAttack = canMove = flag;
    }

    private void OnDestroy() {
        input.Dispose();
    }


    public void Save() {
        SaveObject<int> saveObject = new SaveObject<int>();
        saveObject.AddAll(upgradeLevels);
        SaveManager.SaveSaveObject(SaveManager.GetSaveString(filePath), saveObject);
        print("saved");
    }

    public void LoadLevels() {
        upgradeLevels = SaveManager.LoadSaveObject<int>(SaveManager.GetSaveString(filePath))?.GetDictionary();
        foreach (AbilityController controller in abilities) {
            if (upgradeLevels != null && upgradeLevels.TryGetValue(controller.fileString, out _)) {
                controller.SetUpgradeLevel(upgradeLevels[controller.fileString]);
            }
        }
    }

    public void UpdateUpgradeLevels() {
        foreach (AbilityController controller in abilities) {
            upgradeLevels[controller.fileString] = controller.GetUpgradeLevel();
        }
    }

    public void SwitchScenes() {
        if (carryable != null) {
            isCarrying = false;
            canAttack = true;
            carryable = null;
        }
    }

    public IEnumerator BackToSaveZone() {
        float time = 0.5f;
        SetMobility(false);
        GameManager.sceneAnimator.FadeOut(time);
        Vector3 freezePos = transform.position;
        float timeElapsed = 0;
        while (timeElapsed < time) {
            timeElapsed += Time.deltaTime;
            transform.position = freezePos;
            yield return null;
        }

        transform.position = lastSafePosition;
        GameManager.sceneAnimator.FadeIn(time);
        SetMobility(true);
    }

    public void setHInput(float f) {
        hInput = f;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag(GameManager.Constants.INSTANT_DAMAGE_TAG)) {
            StartCoroutine(BackToSaveZone());
        }

        if (collision.otherCollider == bottomCollider) {
            foreach (AbilityController controller in abilities) controller.OnPlayerCollisionEnter2D(collision);
        }

        PickupCarryable(collision);
    }

    public void OnCollisionExit2D(Collision2D collision) {
        if (collision.otherCollider == bottomCollider) {
            if (collision.gameObject.layer == GameManager.Constants.STANDABLE_LAYER) {
                isInAir = true;
                animator.SetBool(walkString, false);
            }

            foreach (AbilityController controller in abilities) controller.OnPlayerCollisionExit2D(collision);
        }
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (collision.otherCollider == bottomCollider) {
            if (collision.gameObject.CompareTag(GameManager.Constants.STANDABLE_TAG)) {
                lastSafePosition = transform.position;
                if (isInAir) {
                    isInAir = false;
                    animator.SetBool(fallString, false);
                }
            }

            foreach (AbilityController controller in abilities) controller.OnPlayerCollisionStay2D(collision);
        }

        PickupCarryable(collision);
    }
}