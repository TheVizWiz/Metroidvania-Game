using System;
using UnityEngine;

public abstract class IEnemyState : IState {

    protected Animator animator;
    protected Enemy enemy;

    public IEnemyState (Enemy enemy) {
        this.enemy = enemy;
        animator = enemy.GetComponent<Animator>();
    }
}