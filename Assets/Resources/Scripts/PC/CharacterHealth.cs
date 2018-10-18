using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour {

    public int maxHealth;
    private int health;

	// Use this for initialization
	void Start () {
        health = maxHealth;
	}
	
	public void Heal(int healAmount)
    {
        health += healAmount;
        if (health > maxHealth)
            health = maxHealth;
    }

    public void Damage(int hitAmount)
    {
        health -= hitAmount;
        Debug.Log("You took " + hitAmount + " damage.");
        if (health <= 0)
            OnDefeat();
    }

    void OnDefeat()
    {
        gameObject.SetActive(false);
    }
}
