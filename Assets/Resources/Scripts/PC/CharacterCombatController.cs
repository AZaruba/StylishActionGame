using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is not a MonoBehavior. Ideally, only the master would be, but that's for later refactoring
public class CharacterCombatController {

    StateMachine stateMach;
    Weapon currentWeapon;
    // Weapon currentSpecialWeapon

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
                if (currentWeapon.AttackFinished())
                {
                    Command(CommandId.RESET);
                    currentWeapon.ReadyTree();
                }
                else
                {
                    currentWeapon.UpdateAttackTimer();
                }
                break;
            }
            case (StateId.ATTACK_QUEUED): // is this okay if we queue WHILE attacking? This determines whether this implementation works or not
            {
                if (currentWeapon.AttackReady())
                {
                    Command(CommandId.START_ATTACK);
                    currentWeapon.StartAttack();
                }
                else
                {
                    currentWeapon.UpdateAttackTimer();
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
        stateMach.LinkStates(StateId.ATTACKING, StateId.ATTACK_QUEUED, CommandId.QUEUE_ATTACK);
        stateMach.LinkStates(StateId.ATTACKING, StateId.READY, CommandId.INTERRUPT); // function TBD

        stateMach.AddPauseState();
    }

    // starting with a single attack here, the most basic unit
    private void EquipDefaultWeapon()
    {
        /* Building the tree
         * 1. Build all the attacks
         * 2. Create the tree with the default attack
         * 3. Add all child attacks for the current node
         * 4. Move down in the desired direction
         * 5. Repeate 3 and 4 until the combo is done
         * 6. Move back to the root (or up a node) and start again
         * 7. Equip the weapon
         */

        Attack demoHit = new Attack(1, 1, 2); // 1
        Attack demoCombo = new Attack(2, 2, 10);

        Weapon defaultWeapon = new Weapon(demoHit); // 2
        defaultWeapon.MoveTree(Direction.COMBO); // 3
        defaultWeapon.AddAttack(demoCombo, Direction.COMBO); // 4 no 5 currently
        defaultWeapon.ReadyTree(); // 6

        currentWeapon = defaultWeapon; // 7
    }
}
