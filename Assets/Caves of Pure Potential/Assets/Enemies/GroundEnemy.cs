using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace Caves_of_Pure_Potential.Assets.Enemies {
    public class GroundEnemy : StateMachine {

        public new Transform transform;
        [SerializeField] private float moveSpeed;
        private Animator animator;
        private Enemy enemy;

        public List<Vector3> positions;
        [HideInInspector] public Vector3 target;

        protected override void Start() {
            base.Start();
            animator = GetComponent<Animator>();
            transform = base.transform;
            enemy = GetComponent<Enemy>();
            SetTarget(0);

            IdleWaitState waitState = new IdleWaitState(2, 1, animator);
            FindTargetState findTargetState = new FindTargetState(this);
            MoveToTargetState moveToTargetState = new MoveToTargetState(this, moveSpeed);

            SetStartState(waitState);

            AddState("WaitState", waitState);
            AddState("TargetingState", findTargetState);
            AddState("MoveToTargetState", moveToTargetState);

            Transition waitToFindTarget = new Transition(waitState, findTargetState, () => waitState.waited);
            
            Transition findTargetToMove = new Transition(findTargetState, moveToTargetState, () => findTargetState.hasTarget);
            
            Transition moveToWait = new Transition(moveToTargetState, waitState, () => moveToTargetState.lerpAmount >= 1);

            transitions.Add(waitToFindTarget);
            transitions.Add(findTargetToMove);
            transitions.Add(moveToWait);
            // print(transitions.Count);

        }

        protected override void Update() {
            base.Update();
            
            // print(transitions[0].CheckCondition());
            
        }

        public void SetTarget(int i) {
            target = positions[i];
        }
    }

    internal  class IdleWaitState : IState {

        private Animator animator;
        
        private float originalWaitTime;
        private float randomAmount;
        private float waitTime;

        public bool waited;

        public IdleWaitState(float originalWaitTime, float randomAmount, Animator animator) {
            this.originalWaitTime = originalWaitTime;
            this.randomAmount = randomAmount;
            this.animator = animator;
            waited = false;
        }

        public override void OnEnter() {
            // Debug.Log("Idle entered");
            waitTime = originalWaitTime + Random.Range(-randomAmount, randomAmount);
            waited = false;
            animator.SetBool("Waiting", true);
        }

        public override void OnExit() {
            waited = false;
            animator.SetBool("Waiting", false);
            // Debug.Log("Idle left");
        }

        public override void Update() {
            waitTime -= Time.deltaTime;
            // Debug.Log(waited);
            waited = waitTime <= 0;
        }
    }

    internal class FindTargetState : IState {
        
        private List<Vector3> positions;
        private Vector3 currentPos;
        private GroundEnemy groundEnemy;
        private Enemy enemy;

        public bool hasTarget;
        
        public FindTargetState(GroundEnemy groundEnemy) {
            this.positions = groundEnemy.positions;
            this.groundEnemy = groundEnemy;
            this.enemy = groundEnemy.GetComponent<Enemy>();
            hasTarget = false;
        }

        public override void OnEnter() {
            // Debug.Log("Find entered");
            currentPos = enemy.transform.position;

            float maxDistance = 0;
            int x = Random.Range(0, positions.Count - 1);

            while (Mathf.Approximately(positions[x].x,currentPos.x)) {
                x = Random.Range(0, positions.Count - 1);
            }

            groundEnemy.SetTarget(x);
            hasTarget = true;

        }

        public override void OnExit() {
            // Debug.Log("Find left");
            hasTarget = false;
        }

        public override void Update() {
        }

        }

    internal class MoveToTargetState : IState {

        private Vector3 startPosition, endPosition;
        private GroundEnemy groundEnemy;
        private Enemy enemy;
        private float moveSpeed;
        private float distance;
        private float moveTime;

        public float lerpAmount;

        public MoveToTargetState(GroundEnemy groundEnemy, float moveSpeed) {
            this.groundEnemy = groundEnemy;
            this.enemy = groundEnemy.GetComponent<Enemy>();
            lerpAmount = 0;
            this.moveSpeed = moveSpeed;
        }
        public override void OnEnter() {
            startPosition = groundEnemy.transform.position;
            endPosition = groundEnemy.target;
            lerpAmount = 0;
            distance = Mathf.Abs(endPosition.x - startPosition.x);
            // Debug.Log("Move entered " + distance);
            moveTime = 0;
        }

        public override void OnExit() {
            // Debug.Log("Move Left " + distance / moveTime);
            lerpAmount = 0;
            distance = 0;
        }

        public override void Update() {
            lerpAmount += moveSpeed * Time.deltaTime / distance;// * Time.deltaTime;
            // Debug.Log(lerpAmount);
            enemy.body.MovePosition(new Vector2(Vector3.Lerp(startPosition, endPosition, lerpAmount).x, startPosition.y));
            moveTime += Time.deltaTime;


        }
        
        
        
    }
    
    // [CustomEditor(typeof(GroundEnemy))]
    // internal class GroundEnemyEditor : Editor {
    //
    //     public override void OnInspectorGUI() {
    //         base.OnInspectorGUI();
    //     }
    //
    //     [DrawGizmo(GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.Selected)]
    //     public static void DrawGizmos(GroundEnemy enemy, GizmoType type) {
    //         foreach (Vector3 vector3 in enemy.positions) {
    //             Gizmos.color = Color.green;
    //             Gizmos.DrawSphere(vector3, 0.05f);
    //         }
    //     }
    // }
    
}