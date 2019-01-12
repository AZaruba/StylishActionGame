using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationarySphere : MonoBehaviour, IEnemy {

    private int health;
    public int attackDamage;
    public float moveSpeed;

    private Rigidbody rBody;

    // Use this for initialization
    void Start () {
        health = 21;
        // isGettingHit = false;

        rBody = gameObject.GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        rBody.MovePosition(transform.position + CalculateMovement());
	}

    public void GetHit(int damage)
    {
        // if (isGettingHit)
        //     return;

        // isGettingHit = true;

        LoadHitTexture(); // TODO remove
        StartCoroutine(Stun(1.0f));

        health -= damage;
        if (health <= 0)
        {
            this.OnDefeat();
        }

        ResetTexture(); // TODO remove
    }

    
    public void ResetHit()
    {
        // isGettingHit = false;
    }

    public void Attack()
    {
		
    }

    public Vector3 CalculateMovement()
    {
		return Vector3.zero;
    }

    public void OnDefeat()
    {
        Destroy(gameObject);
    }

    public void OnPlayerDefeated()
    {
        // far future goal: animations here
        moveSpeed = 0;
    }
    public Vector3 SendPosition()
    {
        return transform.position;
    }

    /* Stun: Animation Coroutine
     * Allows enemies to be immobilized by attacks. In the future this will enable 
     * knockback, as the normal movement calculation will be short circuited.
     */ 
    private IEnumerator Stun(float stunTime)
    {
        float time = stunTime;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return null;
        }
        yield return null;
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
