﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMasterController : MonoBehaviour {

    // eventually this class will call all other character controllers' FixedUpdate() in this FixedUpdate()
    public CharacterMovementController movementController;
    public GravityController gravityController;
    private StateMachine stateMach;
    private StateId currentStateId;

	// Use this for initialization
	void Start () {
        InitializeStateMachine();
	}
	
	// NOTE FOR LATER: If each object is governed by a state machine, elegant pausing is easy
	void FixedUpdate () {
        // in here we would check state and call the corresponding functions here, via a switch
        UpdateStateMachine();

        switch (currentStateId)
        {
            case (StateId.IDLE):
            {
                // do nothing
                break;
            }
            case (StateId.MOVING):
            {
                movementController.HorizontalMovement(GetMovementStickPosition());
                break;
            }
            case (StateId.IN_AIR):
            {
				gravityController.VerticalMovement();
                break;
            }
            case (StateId.MOVING_IN_AIR):
            {
				gravityController.VerticalMovement();
                movementController.HorizontalMovement(GetMovementStickPosition());
                break;
            }
        }
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
        stateMach.LinkStates(StateId.IDLE, StateId.MOVING, CommandId.MOVE, CommandId.STOP);
		stateMach.LinkStates(StateId.IDLE, StateId.IN_AIR, CommandId.JUMP, gravityController.jumpVelocity);
		stateMach.LinkStates(StateId.IN_AIR, StateId.IDLE, CommandId.LAND);

        stateMach.LinkStates(StateId.IN_AIR, StateId.MOVING_IN_AIR, CommandId.MOVE, CommandId.STOP);
		stateMach.LinkStates(StateId.MOVING, StateId.MOVING_IN_AIR, CommandId.JUMP, gravityController.jumpVelocity);
		stateMach.LinkStates(StateId.MOVING_IN_AIR, StateId.MOVING, CommandId.LAND);
    }

    private void UpdateStateMachine()
    {
        switch (currentStateId)
        {
            case (StateId.IDLE):
            {
                if (GetMovementStickPosition() != Controls.neutralStickPosition)
                {
                    stateMach.CommandMachine(CommandId.MOVE);
                }
                if (Input.GetKey(Controls.Jump))
                {
					float jumpVel = stateMach.CommandMachine(CommandId.JUMP);
					gravityController.StartJump(jumpVel);
                }
                break;
            }
            case (StateId.MOVING):
            {
                if (GetMovementStickPosition() == Controls.neutralStickPosition)
                {
                    stateMach.CommandMachine(CommandId.STOP);
                }
                if (Input.GetKey(Controls.Jump))
                {
                    float jumpVel = stateMach.CommandMachine(CommandId.JUMP);
					gravityController.StartJump(jumpVel);
                }
				// do a fall check then transition to IN AIR with velocity 0 
                break;
            }
            case (StateId.IN_AIR):
            {
                if (GetMovementStickPosition() != Controls.neutralStickPosition)
                {
                    stateMach.CommandMachine(CommandId.MOVE);
                }
                // figure out how to check if we land
				if (gravityController.IsGrounded())
                {
                    stateMach.CommandMachine(CommandId.LAND);
                }
                break;
            }
            case (StateId.MOVING_IN_AIR):
            {
                if (GetMovementStickPosition() == Controls.neutralStickPosition)
                {
                    stateMach.CommandMachine(CommandId.STOP);
                }
				if (gravityController.IsGrounded())
                {
                    stateMach.CommandMachine(CommandId.LAND);
                }
                break;
            }
        }
        currentStateId = stateMach.GetCurrentStateId();
    }

    private float GetMovementStickPosition()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > Controls.deadZone || Mathf.Abs(Input.GetAxis("Horizontal")) > Controls.deadZone)
            return Mathf.Atan2(Input.GetAxis("Vertical"), -1 * Input.GetAxis("Horizontal"));
        return Controls.neutralStickPosition; // return a value well outside of the range of Atan2
    }
}