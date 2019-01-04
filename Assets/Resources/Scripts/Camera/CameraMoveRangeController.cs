using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveRangeController : MonoBehaviour {

    private StateMachine stateMach;

	// Use this for initialization
	void Start ()
    {
        InitializeStateMachine();
	}

    public StateId GetCurrentState()
    {
        return stateMach.GetCurrentStateId();
    }

    private void OnTriggerEnter(Collider other)
    {
        stateMach.CommandMachine(CommandId.COLLIDE);
    }

    private void OnTriggerExit(Collider other)
    {
        stateMach.CommandMachine(CommandId.RESET);
    }

    private void InitializeStateMachine()
    {
        stateMach = new StateMachine(StateId.IDLE);

        stateMach.AddState(StateId.COLLIDING);

        stateMach.LinkStates(StateId.IDLE, StateId.COLLIDING, CommandId.COLLIDE);
        stateMach.LinkStates(StateId.COLLIDING, StateId.IDLE, CommandId.RESET);
    }
}
