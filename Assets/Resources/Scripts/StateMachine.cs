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
    private List<State> states;

	public StateMachine()
	{
        states = new List<State>();
		CreateErrorState();
		defaultState = new State();
		currentState = defaultState;
        states.Add(defaultState);

	}

    public StateMachine(StateId defStateId)
    {
        states = new List<State>();
        State defState = new State(defStateId);
        CreateErrorState();
        defaultState = defState;
        currentState = defaultState;
        states.Add(defaultState);
    }

	public float CommandMachine(CommandId command)
	{
		if (currentState.CheckValidCommand(command))
		{
			float commandVal = currentState.GetCommandValue(command);
			currentState = currentState.GetNextState(command);
			return commandVal;
		}
		return Mathf.NegativeInfinity;
	}

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

    // certain states might need toggling (such as moving and stopping), so this 
    // overload allows the commands from one state to the next and back in a single function
    public bool LinkStates(StateId currentStateId, StateId nextStateId, CommandId command, CommandId commandBack)
    {
        State currentState = FindState(currentStateId);
        State nextState = FindState(nextStateId);
        if (currentState == null || nextState == null)
        {
            return false; // function works iff both states exist
        }

        currentState.AddTransition(nextState, command);
        nextState.AddTransition(currentState, commandBack);
        return true;
    }

    // some situations require ANY state to return to one state (such as waiting out an attack animation)
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
}

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
	public float GetCommandValue(CommandId command)
	{
		for (int x = 0; x < validCommands.Count; x++) // is foreach better?
		{
			if (command == validCommands[x].GetCommandId())
				return validCommands[x].GetCommandValue();
		}
		return Mathf.NegativeInfinity;
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
}

public class Command
{
	private CommandId commandId;
	private float commandValue;

	public Command()
	{
		commandId = CommandId.ERROR_COMMAND;
		commandValue = Mathf.NegativeInfinity;
	}

	public Command(CommandId id)
	{
		commandId = id;
		commandValue = Mathf.NegativeInfinity;
	}

	public Command(CommandId id, float value)
	{
		commandId = id;
		commandValue = value;
	}


    public CommandId GetCommandId()
	{
		return commandId;
	}
	public float GetCommandValue()
	{
		return commandValue;
	}

}

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

    // attack commands
    WAIT, // combos will be based on when the button is pressed during animations
    WAIT_LONG, // "long" wait refers to ending a combo early (some combos are activated by waiting before a press)
    ATTACK_BACK, // pointing the stick toward or away will change attacks
    ATTACK_FORWARD,

    // Pause command
    PAUSE
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

    // Attack states
    COMBO_1_1, // jab (X)
    COMBO_1_2, // punch (X)
    COMBO_1_3, // kick (X)

    COMBO_2_WAIT, // the waiting period between 1_2 and 2_3
    COMBO_2_3, // sweep (wait, X)
    COMBO_2_4, // explosion (X)

    COMBO_3_1, // flip (back, X)
    COMBO_3_2, // launch (X)
}