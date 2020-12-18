using System.Collections;
using UnityEngine;

public class DashController : AbilityController {

    [SerializeField] private ParticleSystem particles;
    private ParticleSystem.EmissionModule emissionModule;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private float dashTime;

    [Header("Level 1")] 
    [SerializeField] private float distance;
    [SerializeField] private float level1TimeBetweenActivations;

    [Header("Level 2")] 
    [SerializeField] private float level2TimeBetweenActivations;

    [Header("Level 3")] 
    [SerializeField] private float level3timeBetweenExplosions;
    [SerializeField] private float level3EnemyDamage;
    [SerializeField] private float level3ChargeTime;
    [SerializeField] private float level3ExplosionRadius;

    [Header("Level 4")] 
    [SerializeField] private float level4EnemyDamage;
    [SerializeField] private float level4ExplosionRadius;

    [Header("Level 5")]
    [SerializeField] private float dazeTimer;
    [SerializeField] private float level5TimeBetweenActivations;
    [SerializeField] private float level5TimeBetweenExplosions;

    private Vector2 dashPosition;
    private Collider2D[] explodedHitEnemyColliders;
    private float distanceDashed;
    private float explosionDistance;
    private float timeSinceLastExplosion;
    private bool exploded;



    // Start is called before the first frame update
    private void Start() {   
        Setup();
        emissionModule = particles.emission;
        timeSinceLastActivation = Mathf.Infinity;
        timeSinceLastExplosion = Mathf.Infinity;
        distanceDashed = 0;
        exploded = false;
        isCharging = false;
        isCharged = false;
        explodedHitEnemyColliders = new Collider2D[]{};
        trail.emitting = false;
        emissionModule.enabled = false;
    }

    // CheckDone is called once per frame
    private void Update() {
        if (isActive) {
            if (upgradeLevel >= 3) {
                timeSinceLastExplosion += Time.deltaTime;
                Physics2D.IgnoreLayerCollision(Constants.PLAYER_LAYER, Constants.ENEMY_LAYER);
            }
            // print("ischarging = " + isCharging + ", ischarged = " + isCharged + ", isActive = " + isActive);
            //checking for explosions for upper tiers
            if (!exploded) {
                if (movement.canAttack && Input.GetButton("Dash") && 
                    (Input.GetButtonDown("Strike") || Input.GetAxisRaw("Strike") > Constants.AXIS_SENSE)) {
                    if (((upgradeLevel == 4 || upgradeLevel == 3) &&
                         timeSinceLastExplosion >= level3timeBetweenExplosions) ||
                        (upgradeLevel == 5 && timeSinceLastExplosion >= level5TimeBetweenExplosions)) {
                        exploded = true;
                        timeSinceLastExplosion = 0;
                        Explode(transform.position);
                    }
                }
            }

            //moving the player per frame while active
            float moveAmount = distance / dashTime * Time.deltaTime;
            distanceDashed += movement.lookDirection * moveAmount;
            dashPosition += Vector2.right * (movement.lookDirection * moveAmount);
            body.MovePosition(dashPosition);
            elapsedTime += Time.deltaTime;
            if (elapsedTime > dashTime) {
                Stop();
                Physics2D.IgnoreLayerCollision(Constants.PLAYER_LAYER, Constants.ENEMY_LAYER, false);
            }
            
            
        } else {
            timeSinceLastActivation += Time.deltaTime;
            timeSinceLastExplosion += Time.deltaTime;
        }
    }

    public override bool Activate() {
        if (isActive) return true;

        bool willActivate = false;
        if (!isActive && movement.canMove && Input.GetButtonDown("Dash")) {
            if (upgradeLevel == 0) {
                return false;
            } else if (upgradeLevel == 1 && timeSinceLastActivation >= level1TimeBetweenActivations) {
                willActivate = true;
                isCharging = false;
                isCharged = false;
            } else if (upgradeLevel < 5 && upgradeLevel > 1 && timeSinceLastActivation >= level2TimeBetweenActivations) {
                willActivate = true;
                isCharging = upgradeLevel > 2;
            } else if (upgradeLevel == 5 && timeSinceLastActivation >= level5TimeBetweenActivations) {
                willActivate = true;
                isCharging = true;
            }

            if (willActivate) {
                isActive = true;
                elapsedTime = 0;
                trail.emitting = true;
                emissionModule.enabled = true;
                movement.canMove = false;
                movement.canTurn = false;
                dashPosition = body.position;
                body.velocity = Vector2.zero;
                timeSinceLastActivation = 0;
                return true;
            }
        }

        return false;
    }

    public override bool Stop() {
        elapsedTime = 0;
        distanceDashed = 0;
        isCharged = false;
        isCharging = false;
        exploded = false;
        isActive = false;
        trail.emitting = false;
        emissionModule.enabled = false;
        movement.canTurn = true;
        movement.canMove = true;
        return true;
    }

    public void Explode(Vector3 position) {
        float waitTime = 1.0f;
        float damage = (upgradeLevel >= 4) ? level4EnemyDamage : level3EnemyDamage;
        float explosionRadius = (upgradeLevel >= 4) ? level4ExplosionRadius : level3ExplosionRadius;
        bool chainDamage = upgradeLevel >= 5;

        StartCoroutine(
            Explosion.CreateExplosion(position, explosionRadius, damage, ElementType.Dark, chainDamage,
                GameManager.enemyLayerMask, waitTime));
    }
}