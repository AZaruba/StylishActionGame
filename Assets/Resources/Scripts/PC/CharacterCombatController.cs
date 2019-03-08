using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is not a MonoBehavior. Ideally, only the master would be, but that's for later refactoring
public class CharacterCombatController {

    StateMachine stateMach;
    Weapon currentWeapon;

    public CharacterCombatController()
    {
        InitializeStateMachine();
        EquipDefaultWeapon();
    }

    public void Update()
    {
        switch(stateMach.GetCurrentStateId())
        {
            case (StateId.READY):
            {
                break;
            }
            case (StateId.ATTACKING):
            {
                if (currentWeapon.AttackReady())
                {
                    Command(CommandId.RESET);
                }
                break;
            }
            case (StateId.ATTACK_QUEUED):
            {
                if (currentWeapon.AttackReady())
                {
                    Command(CommandId.START_ATTACK);
                    currentWeapon.StartAttack();
                }
                break;
            }
        }
    }

    public void Command(CommandId command)
    {
        stateMach.CommandMachine(command);
    }

    public bool IsReady()
    {
        return stateMach.CheckCurrentState(StateId.READY);
    }

    private void InitializeStateMachine()
    {
        // charging will be added later
        stateMach = new StateMachine(StateId.READY);

        stateMach.AddState(StateId.ATTACKING);
        stateMach.AddState(StateId.ATTACK_QUEUED);

        stateMach.LinkStates(StateId.READY, StateId.ATTACK_QUEUED, CommandId.QUEUE_ATTACK);
        stateMach.LinkStates(StateId.ATTACK_QUEUED, StateId.ATTACKING, CommandId.START_ATTACK);
        stateMach.LinkStates(StateId.ATTACKING, StateId.READY, CommandId.RESET);
        stateMach.LinkStates(StateId.ATTACKING, StateId.READY, CommandId.INTERRUPT);

        stateMach.AddPauseState();
    }

    private void EquipDefaultWeapon()
    {
        currentWeapon = new Weapon(this);
    }
}
