using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationarySphere : MonoBehaviour, IEnemy {

    private int health;

    // private bool isGettingHit;
    // public int attackDamage;
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
	
	// Update is called once per frame
	void Update () {
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
        // isGettingHit = false;
    }
    

    public void Attack()
    {
        return;
    }

    public Vector3 CalculateMovement()
    {
        Vector3 direction = manager.SharePlayerPosition() - transform.position;

        return direction.normalized * moveSpeed * Time.deltaTime;
    }

    public void OnDefeat()
    {
        Destroy(gameObject);
    }

    public Vector3 SendPosition()
    {
        return transform.position;
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
