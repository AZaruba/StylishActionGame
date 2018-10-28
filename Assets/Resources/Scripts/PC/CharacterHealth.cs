using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour {

    public int maxHealth;
    private int health;

    public HealthBar healthBar;

	// Use this for initialization
	void Start () {
        healthBar = FindObjectOfType<HealthBar>();
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

        healthBar.UpdateHealthBar(1 - (float)health / (float)maxHealth);
        if (health <= 0)
            OnDefeat();
    }

    void OnDefeat()
    {
        gameObject.SetActive(false);
    }
}
