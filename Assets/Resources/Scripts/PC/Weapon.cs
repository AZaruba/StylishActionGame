using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon {

    // The weapon class should store information on attacks, allowing the
    // combat controller to queue up attacks based on commands without
    // the need to know what weapon is in use
    StateTree<Attack> attackTree;
}

public class Attack
{
    Animation anim;
    int damage;
}
