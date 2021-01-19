using System.Collections;
using System.Collections.Generic;
using GeneralEnemyStates;
using Interfaces;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SafeEnemy : EnemyStateMachine {

    [SerializeField] private bool isAggro;
    public Vector2 startPos;
    public Vector2 endPos;

    protected override void Start() {
        base.Start();
        WaitState waitState = new WaitState(this, 0.5f);
        GeneralMoveState moveState = new GeneralMoveState(this, startPos, endPos, 2f, LeanTweenType.easeInOutSine);
        Transition waitToMove = new Transition(waitState, moveState, () => waitState.isFinished);
        Transition moveToWait = new Transition(moveState, waitState, () => moveState.isFinished);
        AddState("wait", waitState);
        AddState("move", moveState);
        SetStartState(waitState);
        transitions.Add(waitToMove);
        transitions.Add(moveToWait);
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
    
    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    public static void DrawGizmos(SafeEnemy enemy, GizmoType type) {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(enemy.startPos, 0.2f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(enemy.endPos, 0.2f);
    }
}

#endif
