using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacterMasterController : MonoBehaviour {

    // [SerializeField] private NewCharacterMovementController movementController;

    private StateMachine characterStateMachine;
	// Use this for initialization
	void Start ()
    {
        InitializeStateMachine();
	}
	
	// FixedUpdate is called once per tick
	void FixedUpdate ()
    {
		
	}

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
