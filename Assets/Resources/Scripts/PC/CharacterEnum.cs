using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEnum : MonoBehaviour {

    public class AttackTimes
    {
        public const float BasicAttackTime = 1.0f;
        public const float BasicFollowupTime = 1.0f;
        public const float BasicFinisherTime = 1.0f;

        public const float AttackLinkTime = 0.5f;
    }

    public class DamageValues
    {
        public const int BasicAttackDamage = 10;
        public const int BasicFollowupDamage = 10;
        public const int BasicFinisherDamage = 20;
    }

    public enum AttackType
    {
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
