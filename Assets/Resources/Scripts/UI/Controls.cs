using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Controls {

    public static KeyCode Jump;
    public static KeyCode Attack;
    public static KeyCode Special;
    public static KeyCode Interact;
    public static KeyCode Dash;

    // menu controls
    public static KeyCode Confirm = KeyCode.JoystickButton0;

    public static void SetJump(KeyCode kc)
    {
        Jump = kc;
    }

    public static void SetAttack(KeyCode kc)
    {
        Attack = kc;
    }

    public static void SetSpecial(KeyCode kc)
    {
        Special = kc;
    }

    public static void SetInteract(KeyCode kc)
    {
        Interact = kc;
    }

    public static void SetDash(KeyCode kc)
    {
        Dash = kc;
    }
}
