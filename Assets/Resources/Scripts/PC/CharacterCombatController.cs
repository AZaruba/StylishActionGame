using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatController : MonoBehaviour {

    public GameObject hitboxObject;

    private StateMachine attackMach;

    // Use this for initialization
    void Start()
    {
        InitializeStateMachine();
    }

    public void AttackCommand(float stickPosition) // the stick will alter the chosen attack in some instances
    {
        if (stickPosition == Controls.neutralStickPosition)
        {
            attackMach.CommandMachine(CommandId.ATTACK);
        }
        else
        {
            attackMach.CommandMachine(CommandId.ATTACK);
        }
    }

    private void InitializeStateMachine()
    {
        attackMach = new StateMachine(StateId.IDLE);

        attackMach.AddState(StateId.COMBO_1_1);
        attackMach.AddState(StateId.COMBO_1_2);
        attackMach.AddState(StateId.COMBO_1_3);

        attackMach.LinkStates(StateId.IDLE, StateId.COMBO_1_1, CommandId.ATTACK, 10.0f);
        attackMach.LinkStates(StateId.COMBO_1_1, StateId.COMBO_1_2, CommandId.ATTACK, 10.0f);
        attackMach.LinkStates(StateId.COMBO_1_2, StateId.COMBO_1_3, CommandId.ATTACK, 15.0f);

        attackMach.LinkAllStates(StateId.IDLE, CommandId.WAIT_LONG);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 9)
            return;

        IEnemy attackingEnemy = collision.gameObject.GetComponent<IEnemy>();
        if (attackingEnemy != null)
            attackingEnemy.Attack();
    }
}
