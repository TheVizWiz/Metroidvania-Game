using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class EnemyStateMachine : StateMachine {
    // Start is called before the first frame update

    [HideInInspector] public Enemy enemy;
    [SerializeField] protected float detectionRadius;

    protected override void Start() {
        enemy = GetComponent<Enemy>();
        base.Start();
    }


    protected override void Update() {
        base.Update();
    }
}
