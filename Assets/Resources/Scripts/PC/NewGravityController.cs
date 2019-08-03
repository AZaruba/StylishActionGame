using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGravityController : MonoBehaviour {

    [SerializeField] private float terminalVelocity;
    [SerializeField] private float weight;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float jumpForceTime;

    private float currentVelocity = 0;
    private bool rising = false;

    public Vector3 Move()
    {
        Vector3 jumpDelta = new Vector3();

        currentVelocity -= weight;
        if (currentVelocity <= terminalVelocity *-1)
        {
            currentVelocity = terminalVelocity * -1;
        }

        jumpDelta.y = currentVelocity * Time.fixedDeltaTime;

        return jumpDelta;
    }

    public void OnLand()
    {
        currentVelocity = 0;
    }

    /*
    public Vector3 StartJump()
    {
        
    }
    */
}
