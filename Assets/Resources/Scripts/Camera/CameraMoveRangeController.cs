using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveRangeController : MonoBehaviour, Entity {

    private StateMachine stateMach;
	private StateId currentStateId;

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
		// pausing should short circuit any updates
		if (stateMach.GetCurrentStateId() == StateId.PAUSE)
		{
			return;
		}

        stateMach.CommandMachine(CommandId.RESET);
    }

    private void OnTriggerExit(Collider other)
    {
		// pausing should short circuit any updates
		if (stateMach.GetCurrentStateId() == StateId.PAUSE)
		{
			return;
		}

        stateMach.CommandMachine(CommandId.FOLLOW_TARGET);
    }

    private void InitializeStateMachine()
    {
        stateMach = new StateMachine(StateId.TARGET_OUTSIDE_RANGE);

        stateMach.AddState(StateId.TARGET_INSIDE_RANGE);

        stateMach.LinkStates(StateId.TARGET_INSIDE_RANGE, StateId.TARGET_OUTSIDE_RANGE, CommandId.FOLLOW_TARGET);
        stateMach.LinkStates(StateId.TARGET_OUTSIDE_RANGE, StateId.TARGET_INSIDE_RANGE, CommandId.RESET);

		stateMach.AddPauseState();
    }

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

	#endregion
}
