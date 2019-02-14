using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatController : MonoBehaviour {

    private StateMachine stateMach;

    // Use this for initialization
    void Start()
    {
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        stateMach = new StateMachine(StateId.READY);

        stateMach.AddState(StateId.ATTACKING);
        stateMach.AddState(StateId.ATTACKING_LONG);
        stateMach.AddState(StateId.ATTACK_QUEUED);
        stateMach.AddState(StateId.NOT_READY);

        stateMach.LinkStates(StateId.READY, StateId.ATTACK_QUEUED, CommandId.QUEUE_ATTACK);
        stateMach.LinkStates(StateId.ATTACK_QUEUED, StateId.ATTACKING, CommandId.START_ATTACK);

        stateMach.LinkStates(StateId.ATTACKING, StateId.ATTACK_QUEUED, CommandId.QUEUE_ATTACK);
        stateMach.LinkStates(StateId.ATTACKING, StateId.ATTACKING_LONG, CommandId.WAIT);

        stateMach.LinkStates(StateId.ATTACKING_LONG, StateId.ATTACK_QUEUED, CommandId.QUEUE_ATTACK);

        stateMach.LinkStates(StateId.READY, StateId.NOT_READY, CommandId.STOP);
        stateMach.LinkStates(StateId.NOT_READY, StateId.READY, CommandId.RESET);

        stateMach.AddPauseState();
    }

    private void UpdateStateMachine()
    {
        
    }

    public void StartAttack()
    {
        stateMach.CommandMachine(CommandId.QUEUE_ATTACK);
    }
}
