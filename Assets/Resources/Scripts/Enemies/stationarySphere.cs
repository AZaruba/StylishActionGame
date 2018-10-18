using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationarySphere : MonoBehaviour, IEnemy {

    private int health;

    private bool isGettingHit;
    public int attackDamage;
    public float moveSpeed;

    private Rigidbody rBody;
    private SceneManager manager;

    // Use this for initialization
    void Start () {
        health = 21;
        // isGettingHit = false;

        rBody = gameObject.GetComponent<Rigidbody>();
        manager = FindObjectOfType<SceneManager>(); // replace with singleton for scene manager
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
        manager.SendAttack(attackDamage);
    }

    public Vector3 CalculateMovement()
    {
        if (isGettingHit)
            return Vector3.zero;

        Vector3 direction = manager.SharePlayerPosition() - transform.position;

        return direction.normalized * moveSpeed * Time.fixedDeltaTime;
    }

    public void OnDefeat()
    {
        Destroy(gameObject);
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
        isGettingHit = true;
        float time = stunTime;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return null;
        }

        isGettingHit = false;
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
