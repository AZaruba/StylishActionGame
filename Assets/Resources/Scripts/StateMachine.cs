using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine {

	/* For each class/object using a StateMachine:
	 * 1) Generate the StateMachine for the object
	 * 2) Generate a list of potential states (refer to StateId enum)
	 * 3) Create links between states (at this point, states already
	 *    exist, so they just need to be "wired" together via commands)
	 * 
	 * Should states be stored in the object itself or the object's StateMachine?
	 */ 

	private State currentState;
	private State defaultState;
	public static State errorState;

	public StateMachine()
	{
		CreateErrorState();
		defaultState = new State();
		currentState = defaultState;
	}

	public bool CommandMachine(Command command)
	{
		if (currentState.CheckValidCommand(command))
		{
			currentState = currentState.GetNextState(command);
			return true;
		}
		return false;
	}

	public State GetCurrentState()
	{
		return currentState;
	}

	private void CreateErrorState()
	{
		errorState = new State();
	}
}

public class State {
	private StateId stateId;
	private List<Command> validCommands; // two lists or one list of pairs?
	private List<State> validStates; // each command has a corresponding resulting state

	// default constructor is for error state
	public State()
	{
		this.stateId = StateId.ErrorState;
	}

	public State(StateId stateId)
	{
		this.stateId = stateId;
	}

	public bool CheckValidCommand(Command command)
	{
		for (int x = 0; x < validCommands.Count; x++) // is foreach better?
		{
			if (command == validCommands [x])
				return true;
		}
		return false;
 	}

	public State GetNextState(Command command)
	{
		if (!validCommands.Contains (command))
			return StateMachine.errorState;
		
		return validStates[validCommands.IndexOf (command)];
	}

	// adding functions
	public void AddState(State newState, Command command)
	{
		if (validCommands.Count != validStates.Count)
		{
			return; // how should we handle a potential mismatch in state/command sizes?
		}
		validCommands.Add(command);
		validStates.Add(newState);
	}
}

/* A list of all available commands for each entity in the game
 * Characters/Entities will have enabled/disabled commands
 * (for example, all characters might be able to move, but only
 * some may be able to jump)
 */ 
public enum Command
{
	ErrorCommand = -1,
	Stop = 0,
	Move,
	Jump,
	Attack,
	Interact,
	GetHit,
	Recover
}
/* A list of all states
 * 
 */ 
public enum StateId
{
	ErrorState = -1, // corresponds to StateMachine.errorState ONLY
	Idle = 0,
	Moving,
	InAir,
	Attacking,
	Interacting,
	MovingInAir,
	AttackingInAir,
	InteractingInAir,
	Damaged,
	Recovering,
	Defeated
}