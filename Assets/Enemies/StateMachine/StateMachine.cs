using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class StateMachine : MonoBehaviour {

    protected List<Transition> transitions;
    protected Dictionary<string, IState> states;
    protected IState activeState;
    protected IState startState;
    protected double randomFloat;
    private Random randomGen;

    protected virtual void Start() { 
        activeState = startState;
        transitions = new List<Transition>();
        states = new Dictionary<string, IState>();
        randomGen = new Random();
        randomFloat = randomGen.NextDouble();
    }

    protected virtual void Update() {

        activeState.Update();

        foreach (Transition transition in transitions) {
            if ((transition.GetStartState() == activeState || transition.GetStartState() == IState.ANY_STATE) && transition.CheckCondition()) {
                randomFloat = randomGen.NextDouble();
                activeState.OnExit();
                activeState = transition.GetEndState();
                activeState.OnEnter();
            }
        }
        
    }

    protected IState GetState(string key) {
        return states[key];
    }

    protected void AddState(string key, IState state) {
        states.Add(key, state);
    }

    protected void SetStartState(IState state) {
        startState = state;
        activeState = state;
        state.OnEnter();
    }
    
    

}