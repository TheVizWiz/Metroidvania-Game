using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CavesFinalBoss {
    public class SpiderBoss : StateMachine {

        private Enemy enemy;
        internal int idleAnimationStateInt;
        internal int attackAnimationState;
        internal float idleWaitTime;
        internal float attackWaitTime;
        internal bool isIdle;
        internal bool isAttacking;
        
        [SerializeField] internal float baseTimeBetweenIdleAnimations;
        [SerializeField] internal float randomTimeBetweenIdleAnimations;
        [SerializeField] internal float baseTimeBetweenAttackAnimations;
        [SerializeField] internal float randomTimeBetweenAttackAnimations;

        protected new void Start() {
            base.Start();
            
            IdleChooseState idleChooseState = new IdleChooseState(this);
            IdleAnimationState idleAnimationState = new IdleAnimationState(this);
            IdleWaitState idleWaitState = new IdleWaitState(this);
            DeathState deathState = new DeathState(this);
            AttackWaitState attackWaitState = new AttackWaitState(this);
            AttackChooseState state = new AttackChooseState(this);
            AttackHeadLaser attackHeadLaser = new AttackHeadLaser(this);
            AttackLegStomp attackLegStomp = new AttackLegStomp(this);
            AttackFourBurst attackFourBurst = new AttackFourBurst(this);

            SetStartState(idleChooseState);




        }

        protected new void Update() {
            base.Update();
            
            
        }

        public void FinishIdleAnimation() {
            idleAnimationStateInt = -1;
        }

        public void OnDestroy() {
            
        }
    }



    internal class IdleChooseState : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public IdleChooseState(SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
            spiderBoss.idleAnimationStateInt = Random.Range(0, 3);
        }

        public override void OnExit() {
        }

        public override void Update() {
        }
    }

    internal class IdleAnimationState : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public IdleAnimationState(SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
            animator.SetInteger("Idle", spiderBoss.idleAnimationStateInt);
            spiderBoss.idleAnimationStateInt = -1;
        }

        public override void OnExit() {
        }

        public override void Update() {
        }
    }

    internal class IdleWaitState : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public IdleWaitState(SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
            spiderBoss.idleWaitTime = spiderBoss.baseTimeBetweenIdleAnimations +
                                      Random.Range(-spiderBoss.randomTimeBetweenIdleAnimations, spiderBoss.randomTimeBetweenIdleAnimations);
        }

        public override void OnExit() {
        }

        public override void Update() {
            spiderBoss.idleWaitTime -= Time.deltaTime;
        }
    }

    internal class DeathState : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public DeathState (SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
        }

        public override void OnExit() {
        }

        public override void Update() {
        }
    }

    internal class AttackWaitState : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public AttackWaitState (SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
        }

        public override void OnExit() {
        }

        public override void Update() {
        }
    }

    internal class AttackChooseState : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public AttackChooseState(SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
            
        }

        public override void OnExit() {
        }

        public override void Update() {
        }
    }

    internal class AttackHeadLaser : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public AttackHeadLaser(SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
        }

        public override void OnExit() {
        }

        public override void Update() {
        }
    }

    internal class AttackLegStomp : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public AttackLegStomp(SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
        }

        public override void OnExit() {
        }

        public override void Update() {
        }
    }

    internal class AttackFourBurst : IEnemyState {
        
        private SpiderBoss spiderBoss;

        public AttackFourBurst(SpiderBoss boss) : base(boss.GetComponent<Enemy>()) {
            this.spiderBoss = boss;
        }
        public override void OnEnter() {
        }

        public override void OnExit() {
        }

        public override void Update() {
        }
    }
    


}