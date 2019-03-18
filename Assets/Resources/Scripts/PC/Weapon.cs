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
    #endregion
}

public class Attack // TODO: after animations, remove monobehaviour
{
    private Animation anim;
    private int damage;
    private float linkTime; // time in animation at which the next attack can start
    private float longLinkTime; // if the combo branches, this is the late start time
    private float chargeTime;
    private bool ready; // FSM necessary as we want to be in one of four states?
    private bool finished; // idle, attacking, link ready, long link ready

    private float currentTime;

    public Attack()
    {
        ready = true;
        finished = true;
        currentTime = 0;
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
        ready = true;
        finished = true;

        damage = d;
        linkTime = lt;
        longLinkTime = llt;
        chargeTime = ct;

        currentTime = 0;
    }

    public void Execute()
    {
        ready = false;
        finished = false;

        StartTimer();
    }

    public bool IsReady()
    {
        return ready;
    }

    public bool IsFinished()
    {
        return finished;
    }

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
            ready = true;
        }
        if (currentTime > longLinkTime)
        {
            finished = true;
            currentTime = 0;
        }
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
        ready = true;
        while (currentTime < longLinkTime) // long link time will be our "end" time. Again, animations will eventually govern whether the attack is finished
        {
            currentTime += Time.fixedDeltaTime;
            yield return null;
        }
        finished = true;
        yield return null;
    }
    #endregion
}
