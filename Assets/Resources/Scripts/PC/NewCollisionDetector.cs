using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCollisionDetector : MonoBehaviour {

    [SerializeField] private LayerMask layer;
    [SerializeField] private Collider objectCollider;

    private RaycastHit hit;
    private bool grounded;
    /// <summary>
    /// Checks for collision in a given direction for a given distance
    /// </summary>
    /// <param name="direction">A unit vector specifying the intended direction in world space</param>
    /// <param name="distance">Specifies the distance the vector is expected to travel</param>
    /// <returns>Returns true if there's a collision, false if none</returns>
	public bool CheckDirection(Vector3 origin, Vector3 direction, float distance)
    {
        return grounded;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1<<collision.gameObject.layer) & layer) != 0)
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & layer) != 0)
        {
            grounded = false;
        }
    }

    /// <summary>
    /// Checks for overlap between the object and anything in the given range
    /// </summary>
    /// <returns></returns>
    public bool CheckOverlap()
    {
        return false;
    }
}
