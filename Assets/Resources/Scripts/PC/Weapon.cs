using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    // The weapon class should store information on attacks, allowing the
    // combat controller to queue up attacks based on commands without
    // the need to know what weapon is in use
    StateMachine stateMach;

    // Use this for initialization
    void Start() {
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        stateMach = new StateMachine(StateId.READY);

        stateMach.AddState(StateId.ATTACKING);

        stateMach.LinkStates(StateId.READY, StateId.ATTACKING, CommandId.ATTACK);
        stateMach.LinkStates(StateId.ATTACKING, StateId.READY, CommandId.RESET);

        stateMach.AddPauseState();
    }
}
