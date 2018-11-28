using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMasterController : MonoBehaviour {

    // eventually this class will call all other character controllers' FixedUpdate() in this FixedUpdate()
    private StateMachine stateMach;
	// Use this for initialization
	void Start () {
        InitializeStateMachine();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// in here we would check state and call the corresponding functions here, via a switch
	}

    private void InitializeStateMachine()
    {
        // generate a default state, so we know what "0" is
        stateMach = new StateMachine(StateId.IDLE); // create a new FSM with the default state

        // add other states by ID (FSM creates states with the given id)
        stateMach.AddState(StateId.MOVING);
        stateMach.AddState(StateId.IN_AIR);
        stateMach.AddState(StateId.MOVING_IN_AIR);

        // create state connections
        stateMach.LinkStates(StateId.IDLE, StateId.MOVING, Command.MOVE, Command.STOP);
        stateMach.LinkStates(StateId.IDLE, StateId.IN_AIR, Command.JUMP, Command.LAND);
        stateMach.LinkStates(StateId.IN_AIR, StateId.MOVING_IN_AIR, Command.MOVE, Command.STOP);
        stateMach.LinkStates(StateId.MOVING, StateId.MOVING_IN_AIR, Command.JUMP, Command.LAND);
    }
}
