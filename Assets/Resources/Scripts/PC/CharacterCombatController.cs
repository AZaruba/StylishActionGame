using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatController : MonoBehaviour {

    Weapon currentWeapon;
	// Use this for initialization
	void Start () {
        currentWeapon = gameObject.GetComponentInChildren<Weapon>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKeyDown(KeyCode.JoystickButton2) && !currentWeapon.IsAttacking())
        {
            currentWeapon.StartAttack();
        }
    }

    public bool IsAttacking()
    {
        return currentWeapon.IsAttacking();
    }
}
