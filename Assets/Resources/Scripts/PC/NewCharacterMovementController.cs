using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacterMovementController : MonoBehaviour {

    [SerializeField] private float moveSpeed;

    private Vector3 facingDirection;
    private float currentMoveSpeed;

    public Vector3 Move(float moveMagnitude, float directionDegree)
    {
        currentMoveSpeed = moveMagnitude * moveSpeed;
        facingDirection = GetDirectionRelativeToCamera(directionDegree);
        Vector3 movementVector = new Vector3();

        movementVector = facingDirection;
        movementVector *= currentMoveSpeed * Time.fixedDeltaTime;

        return movementVector;
    }


    /// <summary>
    /// Projects the movement control's orientation into the
    /// space viewed by the camera, projected down onto a flat plane
    /// </summary>
    /// <returns>The normalized direction projected into camera-ground space</returns>
    private Vector3 GetDirectionRelativeToCamera(float directionDegree)
    {
        Vector3 relativeDirection = new Vector3();
        Vector3 cameraForward = Camera.main.transform.forward;
        // float stickAngle = Utilities.GetMovementStickPosition(); // move to InputBuffer?

        relativeDirection.x = -1 * cameraForward.z * Mathf.Cos(directionDegree) + cameraForward.x * Mathf.Sin(directionDegree);
        relativeDirection.y = 0;
        relativeDirection.z = cameraForward.z * Mathf.Sin(directionDegree) + cameraForward.x * Mathf.Cos(directionDegree);

        return relativeDirection.normalized;
    }

    /// <summary>
    /// Getter for the direction the player should be facing.
    /// This value sould only be edited in this class, but the master
    /// controller needs access to rotate the rigidbody.
    /// </summary>
    /// <returns>The direction we expect the player to be facing</returns>
    public Quaternion GetFacingDirectionRotation()
    {
        return Quaternion.LookRotation(facingDirection);
    }
}
