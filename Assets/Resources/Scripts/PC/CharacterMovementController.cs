using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour {

    public float moveSpeed;
    // private int collisionMask = 1 << 10;
    // private float currentSpeed;

    private Camera mainCam;
    public Rigidbody rBody;
    public GravityController gravity;

    private Vector3 horizontalTranslation;


    // Use this for initialization
    void Start()
    {
        Physics.IgnoreLayerCollision(11, 10); // ignore collisions between player and weapon

        mainCam = Camera.main;

        horizontalTranslation = Vector3.zero;
    }

    public Vector3 HorizontalMovement(float stickDegree)
    {
        // multiple MovePosition calls didn't work, so accumulating vectors and calling it once is a good fix
        Vector3 newPosition = Vector3.zero;

        if (stickDegree != Controls.neutralStickPosition)
        {
            newPosition += WalkInput(stickDegree);
        }

        horizontalTranslation = newPosition;

        newPosition = gravity.ProjectTranslation(newPosition); // move this vertical translation here to better compartmentalize?

        return newPosition;
    }

    private Vector3 WalkInput(float rotationDegree = 0.0f)
    {
        Vector3 direction = new Vector3();
        Vector3 camDirection = GetCameraOrientation();

        direction.x = -1 * camDirection.z * Mathf.Cos(rotationDegree) + camDirection.x * Mathf.Sin(rotationDegree);
        direction.z = camDirection.z * Mathf.Sin(rotationDegree) + camDirection.x * Mathf.Cos(rotationDegree);

        direction.Normalize();
        camDirection.Normalize();

        rBody.MoveRotation(Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)));
        direction *= moveSpeed * Time.fixedDeltaTime;

        return direction;
    }

    private Vector3 GetCameraOrientation()
    {
        Vector3 cameraDirection;
        if (mainCam)
            cameraDirection = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z);
        else
            throw new System.Exception("Camera not found. Attach a camera object to the character to continue.");

        return cameraDirection;
    }

    public Vector3 SendPosition()
    {
        return transform.position;
    }
    public Vector3 SendHorizontalTranslation()
    {
        return horizontalTranslation;
    }
}
