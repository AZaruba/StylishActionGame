using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour {

    public float weightFactor;
    public float jumpVelocity;
    public float terminalVelocity;

    private bool grounded;

    private Vector3 verticalTranslation;
    private Vector3 projectedVector;

    public Collider objectCollider;
    private Vector3 slopeOffset; // position offset by collider size
    private RaycastHit slopeOut;

    // private LevelManager manager;

    // Use this for initialization
    void Start () {
        verticalTranslation = Vector3.zero;
        grounded = false;

        slopeOffset = Vector3.down * objectCollider.bounds.size.y / 2;

        // manager = FindObjectOfType<LevelManager>(); // replace with singleton for scene manager
    }

    public Vector3 VerticalMovement()
    {
        if (grounded)
        {
            verticalTranslation = Vector3.zero;
        }

        else
        {
            // gravity stuff
            verticalTranslation.y -= weightFactor * Time.fixedDeltaTime;
            if (verticalTranslation.y <= terminalVelocity * -1)
            {
                verticalTranslation.y = terminalVelocity * -1;
            }
        }

        return verticalTranslation; 
		// rBody.MovePosition(transform.position + verticalTranslation);
    }

    public Vector3 ProjectTranslation(Vector3 translationIn)
    {
        Vector3 translationOut = translationIn;
        if (Physics.Raycast(transform.position + slopeOffset, Vector3.down, out slopeOut, 0.1f))
        {
            float mag = translationIn.magnitude;
            translationOut = Vector3.ProjectOnPlane(translationIn, slopeOut.normal).normalized * mag;
        }
        return translationOut;
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (layer != 8)
            return;

        if (layer == 8)
        {
            ContactPoint cPoint = collision.contacts[0];
            if (Vector3.Dot(cPoint.normal, Vector3.up) > 0.5)
            {
                grounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (layer == 9)
        {
            ContactPoint cPoint = collision.contacts[0];
            if (Vector3.Dot(cPoint.normal, Vector3.up) > 0.5)
            {
                grounded = true; // jumping off enemies is neat!
            }
        }
        if (layer == 8)
        {
            ContactPoint cPoint = collision.contacts[0];
            if (Vector3.Dot(cPoint.normal, Vector3.up) > 0.5)
            {
                grounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (layer != 8 && layer != 9)
            return;

        if (collision.contacts.Length == 0)
        {
            grounded = false;
        }
    }

    public Vector3 GetVertTranslation()
    {
        return verticalTranslation;
    }
	public bool IsGrounded()
	{
		return grounded;
	}

    // allows jumping to be called by a variety of sources, not just a button press
    // enables code to be reused for non-player entities that might want to jump
	public void StartJump(float jumpVel)
    {
        grounded = false;
        verticalTranslation.y = jumpVel;
    }
}
