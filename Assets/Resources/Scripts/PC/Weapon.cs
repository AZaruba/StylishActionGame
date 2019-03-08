using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon {

    // The weapon class should store information on attacks, allowing the
    // combat controller to queue up attacks based on commands without
    // the need to know what weapon is in use
    StateTree<Attack> attackTree;

    public Weapon(CharacterCombatController parentIn)
    {
        attackTree = new StateTree<Attack>();
    }

    public void StartAttack()
    {

    }

    public bool AttackReady()
    {
        return attackTree.GetCurrentItem().IsReady();
    }
}

public class Attack
{
    Animation anim;
    int damage;
    float linkTime;
    float longLinkTime;
    float chargeTime;
    bool ready; // a state machine isn't necessary, an attack is either ready or not

    public bool IsReady()
    {
        return ready;
    }
}
