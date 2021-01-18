using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralEnemyStates {
    public class WaitState : IEnemyState {

        public bool isFinished;
        private float waitTime;
        private float timeWaited;
        
        public WaitState(EnemyStateMachine stateMachine, float waitTime) : base(stateMachine.enemy) {
            isFinished = false;
            this.waitTime = waitTime;
        }

        public override void OnEnter() {
            isFinished = false;
            timeWaited = 0;
        }

        public override void OnExit() {
        }

        public override void Update() {
            if (!isFinished) {
                timeWaited += Time.deltaTime;
                if (timeWaited >= waitTime) isFinished = true;
            } 
        }
    }

    public class GeneralMoveState : IEnemyState {
        
        private Vector2 startPosition;
        private Vector2 endPosition;
        private LeanTweenType easeType;
        private float moveTime;
        public bool isFinished;

        public GeneralMoveState(EnemyStateMachine stateMachine, Vector2 startPos, Vector2 endPos, float moveTime, LeanTweenType easeType) : base(stateMachine.enemy) {
            startPosition = startPos;
            endPosition = endPos;
            this.moveTime = moveTime;
            this.easeType = easeType;
            isFinished = false;
        }

        public override void OnEnter() {
            isFinished = false;
            Debug.Log("start pos: " + startPosition + "\nend pos: " + endPosition);
            enemy.transform.position = startPosition;
            LeanTween.move(enemy.gameObject, endPosition, moveTime).setEase(easeType)
                .setOnComplete(o => isFinished = true);
        }

        public override void OnExit() {
            endPosition = startPosition;
            startPosition = enemy.transform.position;
        }

        public override void Update() {
        }
    }
}
