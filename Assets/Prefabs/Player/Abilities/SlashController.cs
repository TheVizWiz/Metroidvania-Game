using System;
using Interfaces;
using UnityEngine;


public class SlashController : AbilityController {

    private float[] damage = {
        0, 10, 20, 30, 40, 50
    };

    public GameObject player1;

    [Header("Level 1")] 
    [SerializeField] private Vector2 level1Range;
    [SerializeField] private Vector2 level1Center;
    [SerializeField] private float level1ChargeTime;
    [SerializeField] private float level1TimeBetweenActivations;
    [SerializeField] private float level1ManaCost;

    [Header("Level 2")] 
    [SerializeField] private float level1DazeDamage;
    [SerializeField] private float level2ChargeTime;
    [SerializeField] private float level2TimeBetweenActivations;

    [Header("Level 3")] 
    [SerializeField] private Vector2 level3Range;
    [SerializeField] private Vector2 level3Center;
    [SerializeField] private float level3ThrowForce;

    [Header("Level 5")]
    [SerializeField] private float level5DazeDamage;
    [SerializeField] private float level5ThrowForce;

    [Header("Level 6")] 
    [SerializeField] private float level6TimeBetweenActivations;
    [SerializeField] private float level6ManaCost;

    [Header("Level 7")] 
    [SerializeField] private float level7WeakenDuration;
    [SerializeField] private float level7WeakenPercentage;

    [Header("Level 8")] 
    [SerializeField] private float level8WeakenDuration;
    [SerializeField] private float level8WeakenPercentage;

    private float timeBetweenActivations,
        manaCost,
        dazeDamage,
        throwForce,
        weakenDuration,
        weakenPercentage;

    private Vector2 range, center, chargePosition;
    private Collider2D[] objectsHit;
    private Animator slashAnimator;
    
    
    // Start is called before the first frame update
    private void Start() {
        Setup();
        objectsHit = new Collider2D[100];
        slashAnimator = GetComponent<Animator>();
    }

    // CheckDone is called once per frame
    private void Update() {
        if (isActive) {
            body.MovePosition(chargePosition);
            if (isCharging) {
                print("charging");
                elapsedChargeTime += Time.deltaTime;
                if (Input.GetButtonUp("Slash") || Input.GetAxis("Slash") < Constants.AXIS_SENSE) {
                    if (elapsedChargeTime >= chargeTime) {
                        isCharged = true;
                        isCharging = false;
                    } else {
                        Stop();
                    }
                }
            }

            if (isCharged) {
                print("charged");
                slashAnimator.SetTrigger("Slash");
                Vector2 point = new Vector2(body.position.x + movement.lookDirection * center.x,
                    body.position.y + center.y);
                ;
                int numHit = Physics2D.OverlapBoxNonAlloc(point, range, 0, objectsHit,
                    GameManager.slashableLayerMask);
                for (int i = 0; i < numHit; i++) {
                    try {
                        objectsHit[i].GetComponent<ISlashable>().Slash(damage[movement.damageLevel], throwForce,
                            weakenPercentage, weakenDuration, ElementType.Ice);
                        objectsHit[i].GetComponent<Enemy>().Daze(dazeDamage);
                    } catch (MissingComponentException e) {
                        print(e.Message);
                    }
                }

                isCharging = false;
                isCharged = false;
            }
        } else {
            timeSinceLastActivation += Time.deltaTime;
        }

    }

    public override bool Activate() {
        if (isActive) return true;
        if (!(upgradeLevel > 0)) return false;
        

        if (movement.canAttack && timeSinceLastActivation >= timeBetweenActivations && 
            (Input.GetButtonDown("Slash") || Input.GetAxisRaw("Slash") > Constants.AXIS_SENSE)) {
            if (movement.isInAir && upgradeLevel <= 2) return false;
            
            
            //charge time block
            if (upgradeLevel >= 4) {
                chargeTime = 0;
                isCharged = true;
            } else if (upgradeLevel >= 2) {
                chargeTime = level2ChargeTime;
                isCharged = false;
                isCharging = true;
            } else {
                chargeTime = level1ChargeTime;
                isCharged = false;
                isCharging = true;
            }
            chargePosition = body.position;
            
            
            //mana cost block
            if (upgradeLevel >= 6) {
                manaCost = level6ManaCost;
            } else {
                manaCost = level1ManaCost;
            }
            
            
            //time between activations block
            if (upgradeLevel >= 6) {
                timeBetweenActivations = level6TimeBetweenActivations;
            } else if (upgradeLevel >= 2) {
                timeBetweenActivations = level2TimeBetweenActivations;
            } else {
                timeBetweenActivations = level1TimeBetweenActivations;
            }
            
            //daze damage block
            if (upgradeLevel >= 5) {
                dazeDamage = level5DazeDamage;
            } else {
                dazeDamage = level1DazeDamage;
            }
            
            //range block
            if (upgradeLevel >= 3) {
                range = level3Range;
                center = level3Center;
            } else {
                range = level1Range;
                center = level1Center;
            }
            
            //throw force block
            if (upgradeLevel >= 5) {
                throwForce = level5ThrowForce;
            } else if (upgradeLevel >= 3) {
                throwForce = level3ThrowForce;
            } else {
                throwForce = 0;
            }
            
            //weaken duration block
            if (upgradeLevel >= 8) {
                weakenDuration = level8WeakenDuration;
            } else if (upgradeLevel >= 7) {
                weakenDuration = level7WeakenDuration;
            } else {
                weakenDuration = 0;
            }
            
            //weaken percentage block
            if (upgradeLevel >= 8) {
                weakenPercentage = level8WeakenPercentage;
            } else if (upgradeLevel <= 7) {
                weakenPercentage = level7WeakenPercentage;
            } else {
                weakenPercentage = 0;
            }

            isActive = true;
            timeSinceLastActivation = 0;
            movement.canMove = false;
            movement.canTurn = false;
            return true;
        }

        return false;
    }

    public override bool Stop() {
        isActive = false;
        movement.canMove = true;
        movement.canTurn = true;
        return true;
    }
}