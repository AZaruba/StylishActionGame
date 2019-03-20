using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon {

    // The weapon class should store information on attacks, allowing the
    // combat controller to queue up attacks based on commands without
    // the need to know what weapon is in use

    // This weapon class has the benefit of being encapsulateed apart from
    // control input, allowing any number of weapons provided available buttons.
    private StateTree<Attack> attackTree;

    public Weapon()
    {
        attackTree = new StateTree<Attack>();
    }

    public Weapon(Attack attack)
    {
        attackTree = new StateTree<Attack>(attack);
    }

    public bool StartAttack()
    {
        if (attackTree.MoveCenter() == false)
        {
            // cancel attack routine if there's nothing next
            return false;
        }

        attackTree.GetCurrentItem().Execute();
        return true;
    }

    public bool AttackReady()
    {
        Attack currentAttack = attackTree.GetCurrentItem();
        if (currentAttack == null)
        {
            return true; // if there's no attack, we are at the root and therefore ready
        }
        return attackTree.GetCurrentItem().IsReady();
    }

    public bool AttackFinished()
    {
        Attack currentAttack = attackTree.GetCurrentItem();
        if (currentAttack == null)
        {
            return true; // if there's no attack, we are at the root and therefore ready
        }
        return attackTree.GetCurrentItem().IsFinished();
    }

    public bool AddAttack(Attack attackIn, Direction dirIn)
    {
        // if there's already an attack at the desired node, fail
        if (attackTree.DoesChildExist(dirIn))
        {
            return false;
        }
        attackTree.SetChildNode(attackIn, dirIn);

        return true;
    }

    public bool MoveTree(Direction dirIn)
    {
        if (!attackTree.DoesChildExist(dirIn))
        {
            return false; // tree does not move
        }

        switch(dirIn)
        {
            case (Direction.LEFT):
            {
                attackTree.MoveLeft();
                break;
            }
            case (Direction.CENTER):
            {
                attackTree.MoveCenter();
                break;
            }
            case (Direction.RIGHT):
            {
                attackTree.MoveRight();
                break;
            }
        }
        return true;
    }

    public void ReadyTree()
    {
        attackTree.ResetTree();
    }

    #region DebugFunctions
    public void UpdateAttackTimer()
    {
        Attack currentAttack = attackTree.GetCurrentItem();
        if (currentAttack == null)
        {
            return;
        }
        currentAttack.UpdateTimer();
    }

    public StateId GetAttackPhase()
    {
        Attack currentAttack = attackTree.GetCurrentItem();
        if (currentAttack == null)
        {
            return StateId.READY;
        }
        return currentAttack.GetAttackPhase();
    }
    #endregion
}

public class Attack // TODO: after animations, remove monobehaviour
{
    private Animation anim;
    private int damage;
    private float linkTime; // time in animation at which the next attack can start
    private float longLinkTime; // if the combo branches, this is the late start time
    private float chargeTime;

    StateMachine attackPhase;

    // TODO : remove below members
    private float resetTime; // should be replaced by a check to our animator

    private float currentTime;

    public Attack()
    {
        currentTime = 0;

        InitializeStateMachine();
    }

    /// <summary>
    /// Constructor for attacks, with default values for a standalone move/finisher
    /// </summary>
    /// <param name="d">Damage value</param>
    /// <param name="lt">Link time. Defaults to -1, which is no link</param>
    /// <param name="llt">Long link time. Defaults to -1, which is no link</param>
    /// <param name="ct">Charge time. Defaults to -1, which is no charge</param>
    public Attack(int d, float lt = -1, float llt = -1, float ct = -1) // Animation anim
    {
        damage = d;
        linkTime = lt;
        longLinkTime = llt;
        chargeTime = ct;

        currentTime = 0;

        InitializeStateMachine();

        resetTime = llt + 1;
    }

    public void Execute()
    {
        Debug.Log(damage); // this is currently our only feedback without an animation
        attackPhase.CommandMachine(CommandId.START_ATTACK);

        StartTimer();
    }

    public bool IsReady()
    {
        return attackPhase.GetCurrentStateId() == StateId.LINK_READY ||
            attackPhase.GetCurrentStateId() == StateId.LATE_LINK_READY;
    }

    // Ensure IsFinished() is never called when the combat controller is ready, as this may cause strange behavior
    public bool IsFinished()
    {
        return attackPhase.GetCurrentStateId() == StateId.READY;
    }

    // Remove the debug functions when animation is in to provide real feedback
    #region DebugFunctions
    public void StartTimer()
    {
        currentTime = 0;
    }

    public void UpdateTimer()
    {
        currentTime += Time.fixedDeltaTime;

        if (currentTime > linkTime)
        {
            attackPhase.CommandMachine(CommandId.READY_LINK);
        }
        if (currentTime > longLinkTime)
        {
            attackPhase.CommandMachine(CommandId.READY_LATE_LINK);
        }
        if (currentTime > resetTime)
        {
            attackPhase.CommandMachine(CommandId.RESET);
            currentTime = 0;
        }
    }

    public StateId GetAttackPhase()
    {
        return attackPhase.GetCurrentStateId();
    }

    // pre-animation function. Will be replaced by using the Animation's current time
    private IEnumerator Timer()
    {
        currentTime = 0;
        while (currentTime < linkTime)
        {
            currentTime += Time.fixedDeltaTime;
            yield return null;
        }
        while (currentTime < longLinkTime) // long link time will be our "end" time. Again, animations will eventually govern whether the attack is finished
        {
            currentTime += Time.fixedDeltaTime;
            yield return null;
        }
        yield return null;
    }
    #endregion

    private void InitializeStateMachine()
    {
        // the attack state machine should be a simple circle
        attackPhase = new StateMachine(StateId.READY);

        attackPhase.AddState(StateId.ANIMATING);
        attackPhase.AddState(StateId.LINK_READY);
        attackPhase.AddState(StateId.LATE_LINK_READY);

        attackPhase.LinkStates(StateId.READY, StateId.ANIMATING, CommandId.START_ATTACK);
        attackPhase.LinkStates(StateId.ANIMATING, StateId.LINK_READY, CommandId.READY_LINK);
        attackPhase.LinkStates(StateId.LINK_READY, StateId.LATE_LINK_READY, CommandId.READY_LATE_LINK);
        attackPhase.LinkStates(StateId.LINK_READY, StateId.READY, CommandId.RESET); // some attacks may not have a late link
        attackPhase.LinkStates(StateId.LATE_LINK_READY, StateId.READY, CommandId.RESET);

        attackPhase.AddPauseState();
    }
}
