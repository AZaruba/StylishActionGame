using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stationarySphere : MonoBehaviour, IEnemy {

    private int health;
    private bool isGettingHit;
    // private int attackDamage;
    // private float moveSpeed;

    // Use this for initialization
    void Start () {
        health = 11;
        isGettingHit = false;
        // attackDamage = 0;
        // moveSpeed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetHit(int damage)
    {
        isGettingHit = true;

        LoadHitTexture(); // TODO remove
        Debug.Log("HIT!");

        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Defeated.");
            this.OnDefeat();
        }

        ResetTexture(); // TODO remove
    }

    public void ResetHit()
    {
        isGettingHit = false;
    }

    public void Attack()
    {
        return;
    }

    public Vector3 CalculateMovement()
    {
        return new Vector3();
    }

    public void OnDefeat()
    {
        Destroy(gameObject);
    }

    /* DEBUG FUNCTIONS
     * The following functions are useful for development purposes, but will be removed
     * as they are replaced with permanent solutions.
     */
    private void LoadHitTexture()
    {
        Texture2D texture;
        texture = (Texture2D)Resources.Load("VisualAssets/Characters/PC/Textures/testCharAttack");

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }

    private void ResetTexture()
    {
        Texture2D texture;
        texture = (Texture2D)Resources.Load("VisualAssets/Characters/PC/Textures/testCharTaunt");

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }
}
