using System;
using UnityEngine;

public abstract class IState {

    public static IState ANY_STATE;

    static IState () {
        ANY_STATE = null;
    }

    protected Func<bool> condition;

    public Func<bool> Condition() {
        return condition;
    }
    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void Update();
}