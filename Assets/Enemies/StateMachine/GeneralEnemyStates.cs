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
        private Vector2 startPos;
        private Vector2 endPos;
        private LeanTweenType easeType;
        private float moveTime;
        public bool isFinished;

        public GeneralMoveState(EnemyStateMachine stateMachine, Vector2 startPos, Vector2 endPos, float moveTime,
            LeanTweenType easeType) : base(stateMachine.enemy) {
            this.startPos = startPos;
            this.endPos = endPos;
            this.moveTime = moveTime;
            this.easeType = easeType;
            isFinished = false;
        }

        public override void OnEnter() {
            isFinished = false;
            Debug.Log("start pos: " + startPos + "\nend pos: " + endPos);
            enemy.transform.position = startPos;
            LeanTween.move(enemy.gameObject, endPos, moveTime).setEase(easeType)
                .setOnComplete(o => isFinished = true);
        }

        public void SetPositions (Vector2 startPos, Vector2 endPos) {
            this.startPos = startPos;
            this.endPos = endPos;
        }

        public override void OnExit() {
            endPos = startPos;
            startPos = enemy.transform.position;
        }

        public override void Update() {
        }
    }
}