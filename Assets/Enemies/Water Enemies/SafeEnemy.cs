using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SafeEnemy : EnemyStateMachine {

    [SerializeField] private bool isAggro;

    protected override void Start() {
        base.Start();
        IdleState locateIdleState = new IdleState(enemy, this);
        AddState("locate state", locateIdleState);
        SetStartState(locateIdleState);

        // Transition transition = new Transition(locateIdleState, null, () =>
            // );
    }

    protected override void Update() {
        base.Update();
        
    }
}

internal class IdleState : IEnemyState {

    private SafeEnemy safeEnemy;
    

    public IdleState(Enemy enemy, SafeEnemy safeEnemy) : base(enemy) {
        this.safeEnemy = safeEnemy;
    }

    public override void OnEnter() {
        
    }

    public override void OnExit() {
    }

    public override void Update() {
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SafeEnemy))]
internal class SafeEnemyCustomEditor: Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
    }
}

#endif
