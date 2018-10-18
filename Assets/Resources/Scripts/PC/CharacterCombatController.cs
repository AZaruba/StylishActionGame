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
        if (Input.GetKeyDown(Controls.Attack) && !currentWeapon.IsAttacking())
        {
            currentWeapon.StartAttack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 9)
            return;

        if (!IsAttacking())
        {
            IEnemy attackingEnemy = collision.gameObject.GetComponent<IEnemy>();
            if (attackingEnemy != null)
                attackingEnemy.Attack();
        }
    }

    public bool IsAttacking()
    {
        return currentWeapon.IsAttacking();
    }
}
