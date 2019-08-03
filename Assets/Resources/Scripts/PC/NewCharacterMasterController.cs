using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacterMasterController : MonoBehaviour {

    [SerializeField] private NewCharacterMovementController movementController;
    [SerializeField] private Rigidbody                      rBody;

    private StateMachine characterStateMachine;
	// Use this for initialization
	void Start ()
    {
        InitializeStateMachine();
	}
	
	// FixedUpdate is called once per tick
	void FixedUpdate ()
    {
        /* I don't want materials-based physics, setting velocities to zero allows the
         * Rigidbody to prevent clipping while not causing any unwanted forces.
         */
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;

        Vector3 newPosition = rBody.position;
        Quaternion newRotation = rBody.rotation;
        // 
        switch (characterStateMachine.GetCurrentStateId())
        {
        case(StateId.IDLE):
            {
                break;
            }
        case(StateId.MOVING):
            {
                newPosition += movementController.Move(InputBuffer.moveMagnitude, Utilities.GetMovementStickPosition());
                newRotation = movementController.GetFacingDirectionRotation();
                break;
            }
        case(StateId.JUMPING):
            {
                break;
            }
        case(StateId.JUMPING_MOVING):
            {
                break;
            }
        }

        rBody.MovePosition(newPosition);
        rBody.MoveRotation(newRotation);
	}

    void LateUpdate()
    {
        UpdateStateMachine();
    }


    /// <summary>
    /// After every update tick we want to update the state machine 
    /// to be ready for the next frame.
    /// </summary>
    private void UpdateStateMachine()
    {
        if (Utilities.GetMovementStickPosition() != Controls.neutralStickPosition)
        {
            characterStateMachine.CommandMachine(CommandId.MOVE);
        }
        else
        {
            characterStateMachine.CommandMachine(CommandId.STOP);
        }

        if (InputBuffer.jumpDown)
        {
            characterStateMachine.CommandMachine(CommandId.JUMP);
        }
        // if new gravity controller is grounded, land
    }

    private void InitializeStateMachine()
    {
        characterStateMachine = new StateMachine(StateId.IDLE);
        characterStateMachine.AddState(StateId.MOVING);
        characterStateMachine.AddState(StateId.JUMPING);
        characterStateMachine.AddState(StateId.JUMPING_MOVING);

        characterStateMachine.LinkStates(StateId.IDLE, StateId.MOVING, CommandId.MOVE);
        characterStateMachine.LinkStates(StateId.IDLE, StateId.JUMPING, CommandId.JUMP);
        characterStateMachine.LinkStates(StateId.MOVING, StateId.IDLE, CommandId.STOP);
        characterStateMachine.LinkStates(StateId.MOVING, StateId.JUMPING_MOVING, CommandId.JUMP);
        characterStateMachine.LinkStates(StateId.JUMPING, StateId.IDLE, CommandId.LAND);
        characterStateMachine.LinkStates(StateId.JUMPING_MOVING, StateId.IDLE, CommandId.LAND);

        characterStateMachine.AddPauseState();
    }
}
