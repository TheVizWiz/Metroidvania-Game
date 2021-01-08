using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using Interfaces;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, ICarrier {
	public static int LOOK_DIRECTION_LEFT = -1, LOOK_DIRECTION_RIGHT = 1; // -1 is left, 1 is right, 0 is nothing - 0 is edge case.

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
	[HideInInspector] public PlayerMain main;
	[HideInInspector] public bool canAttack;
	[HideInInspector] public bool canMove;
	[HideInInspector] public bool canTurn;
	[HideInInspector] public bool canInteract;
	[HideInInspector] public bool isInAir;
	[HideInInspector] public bool isCarrying;
	[HideInInspector] public bool isInUI;
	[HideInInspector] public bool canBashDownwards;
	[HideInInspector] public int lookDirection = 1;
	private int horizontalInput;

	private float timeSinceLastCarry;
	private ICarryable carryable;

	public static readonly string walkString = "Walk";
	public static readonly string fallString = "Fall";
	public static readonly string jumpString = "Jump";
	public static readonly string diveString = "Dive";
	public static readonly string chargeString = "Charge";
	public static readonly string doubleJumpString = "DoubleJump";
	public static readonly string ascendantString = "Ascendant";
	public static readonly string filePath = "abfpth";


	// Start is called before the first frame update
	private void Start() {
		isInAir = false;
		canBashDownwards = false; 
		body = GetComponent<Rigidbody2D>();
		canMove = true;
		canTurn = true;
		canAttack = true;
		isInUI = false;
		if (GameManager.playerMovement == null) {
			GameManager.playerMain = main;
			GameManager.playerMovement = this;
			GameManager.player = this.gameObject;
		}
		DontDestroyOnLoad(gameObject);
		// LoadLevels();
		upgradeLevels = new Dictionary<string, int>();
	}

	// CheckDone is called once per frame
	private void Update() {
		if (Input.GetKeyDown(KeyCode.Keypad3)) {
			UpdateUpgradeLevels();
			Save();
		} else if (Input.GetKeyDown(KeyCode.Keypad6)) {
			LoadLevels();
			print("finished loading levels");
		}

		if (canInteract && !isInAir && Input.GetButtonDown("Interact") && GameManager.dialogueManager.CurrentPosition != DialogueManagerPosition.ShownPosition) {
			GameManager.dialogueManager.Show();
		}

		if (canMove && !isInUI) {
			float hInput = Input.GetAxisRaw("Horizontal");
			if (hInput > Constants.INPUT_ERROR) {
				if (horizontalInput != 1 && !isInAir) {
					animator.SetBool(walkString, true);
				}

				horizontalInput = 1;
			} else if (hInput < -Constants.INPUT_ERROR) {
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

		if (canMove && !isInUI) {
			body.velocity = new Vector2((horizontalInput * moveSpeed), body.velocity.y);
		}

		if (canTurn) {
			SetLookDirection(horizontalInput);
			if (horizontalInput != 0) lookDirection = horizontalInput;
		}



		foreach (AbilityController controller in abilities) {
			if (controller.Activate()) {
				break;
			}
		}
		
		
		if (isCarrying && (Input.GetButtonDown("Strike") || Mathf.Approximately(Input.GetAxis("Strike"), 1))) {
			DropCarryable();
		}

		if (isCarrying && carryable == null) {
			isCarrying = false;
		}

		if (!isCarrying) {
			timeSinceLastCarry += Time.deltaTime;

			if (timeSinceLastCarry >= carryCooldown) {
				Physics2D.IgnoreLayerCollision(Constants.PLAYER_LAYER, Constants.CARRYABLE_LAYER, false);
			}
		}
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
			if (!isCarrying && timeSinceLastCarry >= carryCooldown) {
				carryable.Pickup(transform, carryPosition);
				isCarrying = true;
				canAttack = false;
				this.carryable = carryable;
				Physics2D.IgnoreLayerCollision(Constants.PLAYER_LAYER, Constants.CARRYABLE_LAYER);

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

	public void OnCollisionEnter2D(Collision2D collision) {
		if (collision.otherCollider == bottomCollider) {
			foreach (AbilityController controller in abilities) controller.OnPlayerCollisionEnter2D(collision);
		}

		PickupCarryable(collision);
	}

	public void OnCollisionExit2D(Collision2D collision) {
		if (collision.otherCollider == bottomCollider) {
			if (collision.gameObject.layer == Constants.STANDABLE_LAYER) {
				isInAir = true;
				animator.SetBool(walkString, false);
			}
			foreach (AbilityController controller in abilities) controller.OnPlayerCollisionExit2D(collision);
		}
	}

	public void OnCollisionStay2D(Collision2D collision) {
		if (collision.otherCollider == bottomCollider) {
			if (collision.gameObject.layer == Constants.STANDABLE_LAYER) {
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
