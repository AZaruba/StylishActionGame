﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region StateMachineClass
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
    private List<State> states;

	public StateMachine()
	{
        states = new List<State>();
		CreateErrorState();
		defaultState = new State();
		currentState = defaultState;
        states.Add(defaultState);

		State pauseState = new State(StateId.PAUSE);
		states.Add(pauseState);
	}

    public StateMachine(StateId defStateId)
    {
        states = new List<State>();
        State defState = new State(defStateId);
        CreateErrorState();
        defaultState = defState;
        currentState = defaultState;
        states.Add(defaultState);

		State pauseState = new State(StateId.PAUSE);
		states.Add(pauseState);
    }

    /// <summary>
    /// Commands the state machine with no values provided
    /// </summary>
    /// <param name="command"></param>
    /// <returns>true if transition is successful, false otherwise</returns>
    public bool CommandMachine(CommandId command)
    {
        if (currentState.CheckValidCommand(command))
        {
            currentState = currentState.GetNextState(command);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Commands the state machine with the integer provided
    /// </summary>
    /// <param name="command"></param>
    /// <param name="comInt"></param>
    /// <returns>true if successful, false otherwise. Will give the command's integer value by reference</returns>
    public bool CommandMachine(CommandId command, ref int comInt)
    {
        if (currentState.CheckValidCommand(command))
        {
            comInt = currentState.GetCommandInt(command);
            currentState = currentState.GetNextState(command);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Commands the state machine with no values provided
    /// </summary>
    /// <param name="command"></param>
    /// <param name="comFloat"></param>
    /// <returns>true if successful, false otherwise. Will give the command's float value by reference</returns>
    public bool CommandMachine(CommandId command, ref float comFloat)
    {
        if (currentState.CheckValidCommand(command))
        {
            comFloat = currentState.GetCommandFloat(command);
            currentState = currentState.GetNextState(command);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Commands the state machine with no values provided
    /// </summary>
    /// <param name="command"></param>
    /// <param name="comFloat"></param>
    /// <param name="comInt"></param>
    /// <returns>true if successful, false otherwise. Will give both of the command's values by reference</returns>
    public bool CommandMachine(CommandId command, ref float comFloat, ref int comInt)
	{
		if (currentState.CheckValidCommand(command))
		{
			comFloat = currentState.GetCommandFloat(command);
            comInt = currentState.GetCommandInt(command);
			currentState = currentState.GetNextState(command);
			return true;
		}
		return false;
	}

    /// <summary>
    /// Link currentState to nextState via a transition
    /// </summary>
    /// <param name="currentStateId"></param>
    /// <param name="nextStateId"></param>
    /// <param name="command"></param>
    /// <returns>Returns true if successful, false otherwise</returns>
    public bool LinkStates(StateId currentStateId, StateId nextStateId, CommandId command)
    {
        State currentState = FindState(currentStateId);
        State nextState = FindState(nextStateId);
        if (currentState == null || nextState == null)
        {
            return false; // function works iff both states exist
        }

        currentState.AddTransition(nextState, command);
        return true;
    }

    /// <summary>
    /// Link currentState to nextState via a transition, adds a float value to the transition
    /// </summary>
    /// <param name="currentStateId"></param>
    /// <param name="nextStateId"></param>
    /// <param name="command"></param>
    /// <param name="comValue"></param>
    /// <returns>Returns true if successful, false otherwise</returns>
	public bool LinkStates(StateId currentStateId, StateId nextStateId, CommandId command, float comValue)
	{
		State currentState = FindState(currentStateId);
		State nextState = FindState(nextStateId);
		if (currentState == null || nextState == null)
		{
			return false; // function works iff both states exist
		}

		currentState.AddTransition(nextState, command, comValue);
		return true;
	}

    /// <summary>
    /// Link currentState to nextState via a transition, adds an int value to the transition
    /// </summary>
    /// <param name="currentStateId"></param>
    /// <param name="nextStateId"></param>
    /// <param name="command"></param>
    /// <param name="comValue"></param>
    /// <returns>Returns true if successful, false otherwise</returns>
    public bool LinkStates(StateId currentStateId, StateId nextStateId, CommandId command, int comValue)
    {
        State currentState = FindState(currentStateId);
        State nextState = FindState(nextStateId);
        if (currentState == null || nextState == null)
        {
            return false; // function works iff both states exist
        }

        currentState.AddTransition(nextState, command, comValue);
        return true;
    }

    // some situations require ANY state to return to one state (such as waiting out an attack animation)
    /// <summary>
    /// Links ALL states to a single state, transitioning via a single command.
    /// </summary>
    /// <param name="nextStateId"></param>
    /// <param name="command"></param>
    /// <returns>Returns true if linking is successful, false otherwise</returns>
    public bool LinkAllStates(StateId nextStateId, CommandId command)
    {
        State nextState = FindState(nextStateId);

        if (nextState == null)
        {
            return false;
        }

        for (int x = 0; x < states.Count; x++)
        {
            states[x].AddTransition(nextState, command);
        }
        return true;
    }

	/// <summary>
	/// Adds the paused state to the machine, necessary for classes
	/// implementing the Entity interface. Should be done AFTER adding all states.
	/// </summary>
	public void AddPauseState()
	{
		LinkAllStates(StateId.PAUSE, CommandId.PAUSE);
	}

    public bool CheckCurrentState(StateId id)
    {
        return id == currentState.GetStateId();
    }

	public State GetCurrentState()
	{
		return currentState;
	}
    public StateId GetCurrentStateId()
    {
        return currentState.GetStateId();
    }

    public void AddState(StateId id)
    {
        states.Add(new State(id));
    }

    public State FindState(StateId id)
    {
        for (int x = 0; x < states.Count; x++)
        {
            if (states[x].GetStateId() == id)
            {
                return states[x];
            }
        }
        return null;
    }

	private void CreateErrorState()
	{
		errorState = new State();
	}

	/// <summary>
	/// Forces the state to the current state, iff the state exists.
	/// Use with caution, generally should only be used via unpause.
	/// </summary>
	/// <param name="nextStateId">Next state identifier.</param>
	public void ForceStateChange(StateId nextStateId)
	{
		State nextState = FindState(nextStateId);
        if (nextState != null)
        {
            currentState = nextState;
        }
        else
        {
            currentState = defaultState;
        }
	}
}
#endregion

#region StateClass
public class State {
	private StateId stateId;
	private List<Command> validCommands; // two lists or one list of pairs?
	private List<State> validStates; // each command has a corresponding resulting state

	// default constructor is for error state
	public State()
	{
        validCommands = new List<Command>();
        validStates = new List<State>();
		this.stateId = StateId.ERROR_STATE;
	}

	public State(StateId stateId)
	{
        validCommands = new List<Command>();
        validStates = new List<State>();
        this.stateId = stateId;
	}

	public bool CheckValidCommand(CommandId command)
	{
		for (int x = 0; x < validCommands.Count; x++) // is foreach better?
		{
			if (command == validCommands[x].GetCommandId())
				return true;
		}
		return false;
 	}

	public int GetCommandIndex(CommandId command)
	{
		for (int x = 0; x < validCommands.Count; x++) // is foreach better?
		{
			if (command == validCommands[x].GetCommandId())
				return x;
		}
		return -1;
	}
	public float GetCommandFloat(CommandId command)
	{
		for (int x = 0; x < validCommands.Count; x++) // is foreach better?
		{
			if (command == validCommands[x].GetCommandId())
				return validCommands[x].GetCommandFloat();
		}
		return Utilities.defFloat;
	}
    public int GetCommandInt(CommandId command)
    {
        for (int x = 0; x < validCommands.Count; x++) // is foreach better?
        {
            if (command == validCommands[x].GetCommandId())
                return validCommands[x].GetCommandInt();
        }
        return Utilities.defInt;
    }

    public State GetNextState(CommandId command)
	{
		if (!CheckValidCommand(command))
			return StateMachine.errorState;
		
		return validStates[GetCommandIndex(command)];
	}

    public StateId GetStateId()
    {
        return stateId;
    }

	// adding functions
	public void AddTransition(State newState, CommandId commandId)
	{
		if (validCommands.Count != validStates.Count)
		{
			return; // how should we handle a potential mismatch in state/command sizes?
		}
		validCommands.Add(new Command(commandId));
		validStates.Add(newState);
	}

	public void AddTransition(State newState, CommandId commandId, float commandValue)
	{
		if (validCommands.Count != validStates.Count)
		{
			return; // how should we handle a potential mismatch in state/command sizes?
		}
		validCommands.Add(new Command(commandId, commandValue));
		validStates.Add(newState);
	}

    public void AddTransition(State newState, CommandId commandId, int commandValue)
    {
        if (validCommands.Count != validStates.Count)
        {
            return; // how should we handle a potential mismatch in state/command sizes?
        }
        validCommands.Add(new Command(commandId, commandValue));
        validStates.Add(newState);
    }
}
#endregion

#region CommandClass
public class Command
{
	private CommandId commandId;
	private float commandFloat;
    private int commandInt;

	public Command()
	{
		commandId = CommandId.ERROR_COMMAND;
		commandFloat = Mathf.NegativeInfinity;
	}

	public Command(CommandId id)
	{
		commandId = id;
		commandFloat = Mathf.NegativeInfinity;
	}

	public Command(CommandId id, float value)
	{
		commandId = id;
        commandInt = int.MinValue;
		commandFloat = value;
	}

    public Command(CommandId id, int value)
    {
        commandId = id;
        commandInt = value;
        commandFloat = Mathf.NegativeInfinity;
    }


    public CommandId GetCommandId()
	{
		return commandId;
	}
	public float GetCommandFloat()
	{
		return commandFloat;
	}
    public int GetCommandInt()
    {
        return commandInt;
    }

}
#endregion

#region StateAndCommandIds
/* Using IDs enables classes to create and manage a state machine 
without accessing an entire state or command object */

/* A list of all available commands for each entity in the game
 * Characters/Entities will have enabled/disabled commands
 * (for example, all characters might be able to move, but only
 * some may be able to jump)
 */
public enum CommandId
{
    // player character commands
	ERROR_COMMAND = -1,
	STOP = 0,
	MOVE,
	JUMP,
    FALL,
    LAND,
	ATTACK, // reused between character (issues the attack command to the combat controller) and combat
    DASH,
	INTERACT,
	GET_HIT,
	RECOVER,
    WAIT,

    // camera commands
    FOLLOW,
    STAY_PUT,
    ORBIT,
    RESET,
    LOCK_ON,
    UNLOCK,
    TIME_OUT,
    // WAIT
    // look range commands
    FOLLOW_TARGET,
    // RESET
    
    // combat commands
    // RESET,
    QUEUE_ATTACK,
    CHARGE_ATTACK,
    START_ATTACK,
    INTERRUPT,

    // weapon attacks
    // START_ATTACK
    READY_LINK,
    READY_LATE_LINK,
    // RESET

    // Pause commands
    PAUSE,
	UNPAUSE,
}
/* A list of all states
 * 
 */ 
public enum StateId
{
    // player character states
	ERROR_STATE = -1, // corresponds to StateMachine.errorState ONLY
	IDLE = 0,
	MOVING,
	IN_AIR,
	ATTACKING,
	INTERACTING,
	MOVING_IN_AIR,
	ATTACKING_IN_AIR,
	INTERACTING_IN_AIR,
	DAMAGED,
	RECOVERING,
	DEFEATED,

    // camera states
    STATIONARY,
    FREE_LOOK,
    FOLLOW_TARGET,
    FREE_LOOK_FOLLOW,
    WAITING,
    RETURN_TO_CENTER,
	LOCK_ON,
    // look range states
    TARGET_INSIDE_RANGE,
    TARGET_OUTSIDE_RANGE,

    // combat states
    READY,
    ATTACK_QUEUED,
    // ATTACKING,
    CHARGING,

    // weapon states
    // READY,
    ANIMATING,
    LINK_READY,
    LATE_LINK_READY,

    // pause state
    PAUSE,

    // new state IDs
    // IDLE,
    // MOVING,
    JUMPING,
    JUMPING_MOVING,
    // FALLING,
    // FALLING_MOVING,
    // RECOVERING,
    // IDLE_LONG,
}
#endregion