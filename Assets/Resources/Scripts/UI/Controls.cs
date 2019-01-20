using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Controls {

    public static KeyCode Jump;
    public static KeyCode Attack;
    public static KeyCode Special;
    public static KeyCode Interact;
    public static KeyCode Dash;
	public static KeyCode Pause;

    public static float deadZone;
    public static float neutralStickPosition = -1000;

    // menu controls
    public static KeyCode Confirm = KeyCode.JoystickButton0;

    public static void SetDefaultControls()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                Controls.SetAttack(KeyCode.JoystickButton2);
                Controls.SetJump(KeyCode.JoystickButton0);
                Controls.SetInteract(KeyCode.JoystickButton1);
			    Controls.SetPause(KeyCode.JoystickButton7);
                break;
            case RuntimePlatform.WindowsEditor:
                Controls.SetAttack(KeyCode.JoystickButton2);
                Controls.SetJump(KeyCode.JoystickButton0);
                Controls.SetInteract(KeyCode.JoystickButton1);
			    Controls.SetPause(KeyCode.JoystickButton7);
                break;
		    case RuntimePlatform.OSXPlayer:
		    	Controls.SetAttack(KeyCode.JoystickButton0); // DS4 buttons
	    		Controls.SetJump(KeyCode.JoystickButton1);
	    		Controls.SetInteract(KeyCode.JoystickButton2);
	    		Controls.SetPause(KeyCode.JoystickButton9);
                break;
            case RuntimePlatform.OSXEditor:
                Controls.SetAttack(KeyCode.JoystickButton0); // DS4 buttons
                Controls.SetJump(KeyCode.JoystickButton1);
                Controls.SetInteract(KeyCode.JoystickButton2);
			    Controls.SetPause(KeyCode.JoystickButton9);
                break;
            case RuntimePlatform.LinuxPlayer:
                Controls.SetAttack(KeyCode.JoystickButton2);
                Controls.SetJump(KeyCode.JoystickButton0);
                Controls.SetInteract(KeyCode.JoystickButton1);
                break;
            case RuntimePlatform.LinuxEditor:
                Controls.SetAttack(KeyCode.JoystickButton2);
                Controls.SetJump(KeyCode.JoystickButton0);
                Controls.SetInteract(KeyCode.JoystickButton1);
                break;
        }
        
        Controls.SetDeadzone(0.5f);
    }

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
	public static void SetPause(KeyCode kc)
	{
		Pause = kc;
	}

    public static void SetDeadzone(float dz)
    {
        deadZone = dz;
    }
}
