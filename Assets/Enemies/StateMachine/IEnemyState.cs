using System;
using UnityEngine;

public abstract class IEnemyState : IState {

    protected Enemy enemy;

    public IEnemyState (Enemy enemy) {
        this.enemy = enemy;
    }
}