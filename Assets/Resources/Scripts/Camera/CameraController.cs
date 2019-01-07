using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float heightFromGround;
    [SerializeField] private float distanceFromTarget;
    [SerializeField] private float lookHeightAboveTarget;
    // [SerializeField] private float pitchRotation;
    // [SerializeField] private float horizontalOffset;

    [SerializeField] private float maxVerticalAngle;

    [SerializeField] private float cameraSmoothingFactor;

	[SerializeField] private float resetWaitTime;

    [SerializeField] private GameObject target;
    [SerializeField] private CharacterMasterController characterController;
    [SerializeField] private GameObject freeMoveRange;
    [SerializeField] private CameraMoveRangeController rangeController;

    private StateMachine cameraMach;
	private float timer;

    // Use this for initialization
    void Start ()
    {
        InitializeStateMachine();
        freeMoveRange.transform.position = target.transform.position;

        rangeController = freeMoveRange.GetComponent<CameraMoveRangeController>();
        if (rangeController == null)
        {
            freeMoveRange.AddComponent<CameraMoveRangeController>();
            rangeController = freeMoveRange.GetComponent<CameraMoveRangeController>();
        }

        characterController = target.GetComponent<CharacterMasterController>();
        if (characterController == null)
        {
            target.AddComponent<CharacterMasterController>();
            characterController = target.GetComponent<CharacterMasterController>();
        }
		timer = 0f;
	}

    private void FixedUpdate()
    {
        switch (cameraMach.GetCurrentStateId())
        {
            case (StateId.FREE_LOOK):
            {
                break;
            }
		    case (StateId.FREE_LOOK_AT_CENTER):
			{
				break;
			}
            case (StateId.FOLLOW_TARGET):
            {
                Vector3 delta = characterController.GetPositionDelta();
                transform.position += delta;
                freeMoveRange.transform.position += delta;
                break;
            }
            case (StateId.RETURN_TO_CENTER):
            {
                break;
            }
        }
    }

    private void Update()
    {
        UpdateStateMachine();

        switch (cameraMach.GetCurrentStateId())
        {
            case (StateId.FREE_LOOK):
            {
                // for now doesn't need anything
                Vector3 lookTarget = freeMoveRange.transform.position;
                lookTarget.y += lookHeightAboveTarget;
                transform.LookAt(lookTarget);
				timer += Time.deltaTime;
                break;
            }
		    case (StateId.FREE_LOOK_AT_CENTER):
			{
				// for now doesn't need anything
				Vector3 lookTarget = freeMoveRange.transform.position;
				lookTarget.y += lookHeightAboveTarget;
				transform.LookAt(lookTarget);
				timer = 0f;
				break;
			}
            case (StateId.FOLLOW_TARGET):
            {
				timer = 0f;
                break;
            }
            case (StateId.RETURN_TO_CENTER):
            {
                Vector3 lookTarget = freeMoveRange.transform.position;
                lookTarget.y += lookHeightAboveTarget;
                transform.LookAt(lookTarget);
                MaintainCameraDistance(CalculateRequiredDistance());
                ResetFreeMoveRangePosition(target.transform.position);
                break;
            }
        }
    }

    private void InitializeStateMachine()
    {
        cameraMach = new StateMachine(StateId.FREE_LOOK);

        cameraMach.AddState(StateId.FOLLOW_TARGET);
        cameraMach.AddState(StateId.RETURN_TO_CENTER);
		cameraMach.AddState(StateId.FREE_LOOK_AT_CENTER);

        cameraMach.LinkStates(StateId.FREE_LOOK, StateId.FOLLOW_TARGET, CommandId.FOLLOW);
		cameraMach.LinkStates(StateId.FREE_LOOK_AT_CENTER, StateId.FOLLOW_TARGET, CommandId.FOLLOW);
        cameraMach.LinkStates(StateId.FOLLOW_TARGET, StateId.FREE_LOOK, CommandId.STAY_PUT);
        cameraMach.LinkStates(StateId.RETURN_TO_CENTER, StateId.FOLLOW_TARGET, CommandId.FOLLOW); // if player is on edge of free move range then moves outside again
		cameraMach.LinkStates(StateId.RETURN_TO_CENTER, StateId.FREE_LOOK_AT_CENTER, CommandId.STAY_PUT);
		cameraMach.LinkAllStates(StateId.RETURN_TO_CENTER, CommandId.RESET);

    }

    private void UpdateStateMachine()
    {
        if (rangeController.GetCurrentState() == StateId.TARGET_INSIDE_RANGE)
        {
            if (characterController.GetPositionDelta() == Vector3.zero && Vector3.Distance(transform.position, CalculateRequiredDistance()) > 0.1f
				&& timer > resetWaitTime)
			{
				cameraMach.CommandMachine(CommandId.RESET);
            }
            else
            {
                cameraMach.CommandMachine(CommandId.STAY_PUT);
            }

        }
        else if (rangeController.GetCurrentState() == StateId.TARGET_OUTSIDE_RANGE)
        {
            cameraMach.CommandMachine(CommandId.FOLLOW);
        }
    }

    #region ValueChecks
    // Checking various camera values to ensure the camera is within behavior parameters

    /// <summary>
    /// Checks if the camera's current vertical angle is outside the valid range.
    /// </summary>
    /// <returns>Returns true if with the valid range, false otherwise</returns>
    private bool IsCameraAngleValid()
    {
        Vector3 cameraToTarget = target.transform.position - transform.position;
        Vector3 flatVectorToTarget = cameraToTarget;
        flatVectorToTarget.y = 0; // removes Y (height) component

        float currentCameraAngle = Vector3.Angle(cameraToTarget.normalized, flatVectorToTarget.normalized);
        if (currentCameraAngle > maxVerticalAngle)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Calculates the endpoint for MaintainDistance()
    /// </summary>
    /// <returns>the position the camera needs to be at when MaintainDistance finishes</returns>
    private Vector3 CalculateRequiredDistance()
    {
        // get the direction we want the camera to be in relative to the player
        Vector3 newCameraPosition = target.transform.forward * -1; // we want to be BEHIND, not in front
        newCameraPosition.Normalize();

        // get the height either from the camera or the player
        float height = 0.0f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, heightFromGround, Utilities.environmentOnly))
        {
            height = hit.transform.position.y + heightFromGround;
        }
        else
        {
            height = target.transform.position.y + heightFromGround;
        }
        newCameraPosition *= Mathf.Sqrt(Mathf.Pow(distanceFromTarget,2) - Mathf.Pow(height, 2)); // Pythagorean Theorem always comes in handy!
        newCameraPosition.y = height;
        newCameraPosition += target.transform.position;

        return newCameraPosition;
    }

    #endregion

    #region MovementInterpolation
    /* Sometimes the camera should move without user input.
     * These functions interpolate the movement over time to make it smooth
     */

    private void ResetFreeMoveRangePosition(Vector3 targetPosition)
    {
        freeMoveRange.transform.position = Vector3.Lerp(freeMoveRange.transform.position, targetPosition, cameraSmoothingFactor * Time.deltaTime);
    }

    /// <summary>
    /// Ensures that the camera will always remain at the required distance from target during gameplay
    /// (aside from special circumstances such as set pieces or running into walls)
    /// </summary>
    /// <returns></returns>
    private void MaintainCameraDistance(Vector3 targetPosition)
    {
        transform.position = Vector3.Slerp(transform.position, targetPosition, cameraSmoothingFactor * Time.deltaTime);
    }
    #endregion
}
