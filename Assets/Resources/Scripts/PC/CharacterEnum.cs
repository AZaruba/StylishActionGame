using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEnum : MonoBehaviour {

    public class Attack
    {
        public Attack(float aTime, float lTime, int aDamage, KeyCode key)
        {
            this.attackTime = aTime;
            this.linkTime = lTime;
            this.damage = aDamage;
            this.inputKey = key;

            // xAxis = 0;
            // yAxis = 0;
        }

        public void LinkAttacks(Attack next)
        {
            this.nextAttack = next;
        }

        // Timing data
        public float attackTime;
        public float linkTime;

        // Combat data
        public int damage;
        public Attack nextAttack;

        // Input data
        public KeyCode inputKey;
        // public float xAxis;
        // public float yAxis;

    }

    public enum AttackType
    {
        NoAttack = -1,
        BasicPunch,
        FollowupPunch,
        FinisherPunch,
        Kick,
        Taunt
    }

    public class Controls
    {
        public const KeyCode Jump = KeyCode.JoystickButton0;
        public const KeyCode AttackOne = KeyCode.JoystickButton2;
        public const KeyCode AttackTwo = KeyCode.JoystickButton3;
    }

}
