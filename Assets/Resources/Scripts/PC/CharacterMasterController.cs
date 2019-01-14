using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMasterController : MonoBehaviour, Entity {

    // eventually this class will call all other character controllers' FixedUpdate() in this FixedUpdate()
    [SerializeField] private CharacterMovementController movementController;
    [SerializeField] private GravityController gravityController;
    [SerializeField] private CharacterCombatController combatController;
    [SerializeField] private Rigidbody rBody;

    private StateMachine stateMach;
    private StateId currentStateId;

    private Vector3 positionDelta; // this is the change in player position for camera use

	// Use this for initializationß
	void Start () {
        InitializeStateMachine();
        InitializeVariables();
	}

    // evidently input should be read in update and not fixedUpdate
    void Update()
    {
        UpdateStateMachine();
    }

    // NOTE FOR LATER: If each object is governed by a state machine, elegant pausing is easy
    void FixedUpdate () {

        /* I don't want materials-based physics, setting velocities to zero allows the
         * Rigidbody to prevent clipping while not causing any unwanted forces.
         */
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        switch (stateMach.GetCurrentStateId())
        {
            case (StateId.IDLE):
            {
                positionDelta = Vector3.zero;
                // do nothing
                break;
            }
            case (StateId.MOVING):
            {
                Vector3 newPosition = transform.position;
                positionDelta = movementController.HorizontalMovement(Utilities.GetMovementStickPosition());
                positionDelta = gravityController.ProjectTranslation(positionDelta);
                newPosition += positionDelta;

                rBody.MovePosition(newPosition);
                break;
            }
            case (StateId.IN_AIR):
            {
                Vector3 newPosition = transform.position;
                positionDelta = gravityController.VerticalMovement();
                newPosition += positionDelta;

                rBody.MovePosition(newPosition);
                break;
            }
            case (StateId.MOVING_IN_AIR):
            {
                Vector3 newPosition = transform.position;
                positionDelta = gravityController.VerticalMovement();
                positionDelta += movementController.AerialHorizontalMovement(Utilities.GetMovementStickPosition());
                newPosition += positionDelta;
                rBody.MovePosition(newPosition);
                break;
            }
            case (StateId.ATTACKING):
            {

                break;
            }
        }
	}

    #region StateMachine
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
        stateMach.LinkStates(StateId.IDLE, StateId.MOVING, CommandId.MOVE);
        stateMach.LinkStates(StateId.MOVING, StateId.IDLE, CommandId.STOP);
		stateMach.LinkStates(StateId.IDLE, StateId.IN_AIR, CommandId.JUMP, gravityController.jumpVelocity);
        stateMach.LinkStates(StateId.IDLE, StateId.IN_AIR, CommandId.FALL, 0.0f);
        stateMach.LinkStates(StateId.IN_AIR, StateId.IDLE, CommandId.LAND);

        stateMach.LinkStates(StateId.IN_AIR, StateId.MOVING_IN_AIR, CommandId.MOVE);
        stateMach.LinkStates(StateId.MOVING_IN_AIR, StateId.IN_AIR, CommandId.STOP);
        stateMach.LinkStates(StateId.MOVING, StateId.MOVING_IN_AIR, CommandId.JUMP, gravityController.jumpVelocity);
        stateMach.LinkStates(StateId.MOVING, StateId.MOVING_IN_AIR, CommandId.FALL, 0.0f);
		stateMach.LinkStates(StateId.MOVING_IN_AIR, StateId.MOVING, CommandId.LAND);

        stateMach.LinkStates(StateId.IDLE, StateId.ATTACKING, CommandId.ATTACK);
        stateMach.LinkStates(StateId.MOVING, StateId.ATTACKING, CommandId.ATTACK);
        stateMach.LinkStates(StateId.ATTACKING, StateId.IDLE, CommandId.WAIT_LONG);

		stateMach.AddPauseState();
    }

    private void UpdateStateMachine()
    {
		// pausing should short circuit any updates
		if (stateMach.GetCurrentStateId() == StateId.PAUSE)
		{
			return;
		}

        // consider the order of these if statements in the future
        int comInt = Utilities.defInt;
        float comFloat = Utilities.defFloat;

        if (Utilities.GetMovementStickPosition() != Controls.neutralStickPosition)
        {
            stateMach.CommandMachine(CommandId.MOVE);
        }
        else
        {
            stateMach.CommandMachine(CommandId.STOP);
        }
        if (Input.GetKeyDown(Controls.Jump))
        {
            stateMach.CommandMachine(CommandId.JUMP, ref comFloat, ref comInt);
            if (!float.IsInfinity(comFloat)) // fix up later: Under no circumstances will jumpVel be below -1 but this could be more elegant
            {
                gravityController.StartJump(comFloat);
            }
        }
        else if (!gravityController.IsGrounded())
        {
            stateMach.CommandMachine(CommandId.FALL, ref comFloat, ref comInt);
            if (!float.IsInfinity(comFloat))
            {
                gravityController.StartJump(comFloat);
            }
        }
        else
        {
            stateMach.CommandMachine(CommandId.LAND);
        }
        /* commented out until we get attacking worked out
        if (Input.GetKeyDown(Controls.Attack))
        {
            stateMach.CommandMachine(CommandId.ATTACK);
            combatController.AttackCommand(GetMovementStickPosition());
        }
        */
    }
    #endregion

    private void InitializeVariables()
    {
        positionDelta = Vector3.zero;
    }

    #region GettersAndSetters
    public Vector3 GetPositionDelta()
    {
        return positionDelta;
    }

    public StateId GetCurrentState()
    {
        return stateMach.GetCurrentStateId();
    }
    #endregion

	#region StateSaving
	public void Pause()
	{
		currentStateId = stateMach.GetCurrentStateId();
		stateMach.CommandMachine(CommandId.PAUSE);
	}

	public void Unpause()
	{
		stateMach.ForceStateChange(currentStateId);
	}

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
	#endregion
}
