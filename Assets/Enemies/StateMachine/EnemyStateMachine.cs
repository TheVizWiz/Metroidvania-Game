using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateMachine : StateMachine {
    // Start is called before the first frame update

    protected Enemy enemy;
    [SerializeField] protected float detectionRadius;
    
    protected override void Start() {
        base.Start();
        enemy = GetComponent<Enemy>();
    }


    protected override void Update() {
        base.Update();
    }
}
