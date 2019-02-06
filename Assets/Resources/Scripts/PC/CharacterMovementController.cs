using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour {

    [SerializeField] private float moveSpeed;
    [SerializeField] private float airMoveSpeed;

    private Camera mainCam;
    public Rigidbody rBody;

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
            newPosition += WalkInput(moveSpeed, stickDegree);
        }

        horizontalTranslation = newPosition;
        return newPosition;
    }

    public Vector3 AerialHorizontalMovement(float stickDegree)
    {
        // multiple MovePosition calls didn't work, so accumulating vectors and calling it once is a good fix
        Vector3 newPosition = Vector3.zero;

        if (stickDegree != Controls.neutralStickPosition)
        {
            newPosition += WalkInput(airMoveSpeed, stickDegree);
        }

        horizontalTranslation = newPosition;
        return newPosition;
    }

    private Vector3 WalkInput(float currentMoveSpeed, float rotationDegree = 0.0f)
    {
        Vector3 direction = new Vector3();
        Vector3 camDirection = GetCameraOrientation();
        camDirection.Normalize();

        direction.x = -1 * camDirection.z * Mathf.Cos(rotationDegree) + camDirection.x * Mathf.Sin(rotationDegree);
        direction.z = camDirection.z * Mathf.Sin(rotationDegree) + camDirection.x * Mathf.Cos(rotationDegree);
        rBody.MoveRotation(Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)));
        direction *= currentMoveSpeed * Time.fixedDeltaTime * Utilities.GetMovementStickMagnitude();

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

    /// <summary>
    /// Provides the current orientation of the player character
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCurrentOrientation()
    {
        return Vector3.forward;
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
