using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMasterController : MonoBehaviour {

    // eventually this class will call all other character controllers' FixedUpdate() in this FixedUpdate()
    public CharacterMovementController movementController;
    public GravityController gravityController;
    public CharacterCombatController combatController;
    public Rigidbody rBody;

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

        /* I don't want materials-based physics, setting velocities to zero allows the
         * Rigidbody to prevent clipping while not causing any unwanted forces.
         */
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        switch (currentStateId)
        {
            case (StateId.IDLE):
            {
                // do nothing
                break;
            }
            case (StateId.MOVING):
            {
                Vector3 newPosition = transform.position;
                newPosition += movementController.HorizontalMovement(GetMovementStickPosition());

                rBody.MovePosition(newPosition);
                break;
            }
            case (StateId.IN_AIR):
            {
                Vector3 newPosition = transform.position;
                newPosition += gravityController.VerticalMovement();

                rBody.MovePosition(newPosition);
                break;
            }
            case (StateId.MOVING_IN_AIR):
            {
                Vector3 newPosition = transform.position;
                newPosition += gravityController.VerticalMovement();
                newPosition += movementController.HorizontalMovement(GetMovementStickPosition());

                rBody.MovePosition(newPosition);
                break;
            }
            case (StateId.ATTACKING):
            {

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
        stateMach.AddState(StateId.ATTACKING);

        // create state connections
        stateMach.LinkStates(StateId.IDLE, StateId.MOVING, CommandId.MOVE, CommandId.STOP); // complicated, remove two way transition adding
		stateMach.LinkStates(StateId.IDLE, StateId.IN_AIR, CommandId.JUMP, gravityController.jumpVelocity);
        stateMach.LinkStates(StateId.IDLE, StateId.IN_AIR, CommandId.FALL, 0.0f);
        stateMach.LinkStates(StateId.IN_AIR, StateId.IDLE, CommandId.LAND);

        stateMach.LinkStates(StateId.IN_AIR, StateId.MOVING_IN_AIR, CommandId.MOVE, CommandId.STOP);
		stateMach.LinkStates(StateId.MOVING, StateId.MOVING_IN_AIR, CommandId.JUMP, gravityController.jumpVelocity);
        stateMach.LinkStates(StateId.MOVING, StateId.MOVING_IN_AIR, CommandId.FALL, 0.0f);
		stateMach.LinkStates(StateId.MOVING_IN_AIR, StateId.MOVING, CommandId.LAND);

        stateMach.LinkStates(StateId.IDLE, StateId.ATTACKING, CommandId.ATTACK);
        stateMach.LinkStates(StateId.MOVING, StateId.ATTACKING, CommandId.ATTACK);
        stateMach.LinkStates(StateId.ATTACKING, StateId.IDLE, CommandId.WAIT_LONG);
    }

    private void UpdateStateMachine()
    {
        switch (currentStateId)
        {
            case (StateId.IDLE): // GET RID OF THIS, THE POINT OF A STATE MACHINE IS TO NOT NEED THIS
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
                if (!gravityController.IsGrounded())
                {
                    float jumpVel = stateMach.CommandMachine(CommandId.FALL);
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
                if (!gravityController.IsGrounded())
                {
                    float jumpVel = stateMach.CommandMachine(CommandId.FALL);
                    gravityController.StartJump(jumpVel);
                }
                break;
            }
            case (StateId.IN_AIR):
            {
                if (GetMovementStickPosition() != Controls.neutralStickPosition)
                {
                    stateMach.CommandMachine(CommandId.MOVE);
                }
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
                    Debug.Log("Womp moving");
                    stateMach.CommandMachine(CommandId.LAND);
                }
                break;
            }
        }
        if (Input.GetKey(Controls.Attack))
        {
            stateMach.CommandMachine(CommandId.ATTACK);
            combatController.AttackCommand(GetMovementStickPosition());
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
