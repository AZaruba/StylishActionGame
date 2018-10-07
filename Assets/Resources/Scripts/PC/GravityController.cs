using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour {

    public float weightFactor;
    public float jumpVelocity;
    public float terminalVelocity;

    private bool grounded;
    private bool jumpReady;

    private Vector3 verticalTranslation;

	// Use this for initialization
	void Start () {
        verticalTranslation = Vector3.zero;
        grounded = false;
        jumpReady = false;
	}

    private void FixedUpdate()
    {
        if (grounded)
        {
            verticalTranslation = Vector3.zero;
            if (!jumpReady)
                jumpReady = true;
        }

        else
        {
            // gravity stuff
            if (!jumpReady)
            {
                verticalTranslation.y -= weightFactor * Time.deltaTime;
                if (verticalTranslation.y <= terminalVelocity*-1)
                    verticalTranslation.y = terminalVelocity*-1;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint cPoint = collision.contacts[0];
        if (Vector3.Dot(cPoint.normal, Vector3.up) > 0.5)
        {
            grounded = true;
        }
    }

    public Vector3 GetVertTranslation()
    {
        return verticalTranslation;
    }

    // allows jumping to be called by a variety of sources, not just a button press
    // enables code to be reused for non-player entities that might want to jump
    public void StartJump()
    {
        if (jumpReady)
        {
            jumpReady = false;
            grounded = false;
            verticalTranslation.y = jumpVelocity * Time.deltaTime;
        }
    }
}
