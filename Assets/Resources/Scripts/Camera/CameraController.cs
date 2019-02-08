﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, Entity {

    [SerializeField] private float heightFromGround;
    [SerializeField] private float distanceFromTarget;
    [SerializeField] private float lookHeightAboveTarget;
    // [SerializeField] private float pitchRotation;
    // [SerializeField] private float horizontalOffset;

    [SerializeField] private float maxVerticalAngle;

    [SerializeField] private float cameraInertia;
    [SerializeField] private float cameraMoveSpeed;

	[SerializeField] private float resetWaitTime;

    [SerializeField] private GameObject target;
    [SerializeField] private CharacterMasterController characterController;
    [SerializeField] private GameObject freeMoveRange;
    [SerializeField] private CameraMoveRangeController rangeController;

    private StateMachine cameraMach;
	private StateId currentStateId;
	private float timer;
    private Vector3 lookAboveVector;

    // Use this for initialization
    void Start ()
    {
        InitializeStateMachine();
        lookAboveVector = new Vector3(0, lookHeightAboveTarget, 0);

        Vector3 moveRangePosition = transform.position;
        moveRangePosition.y -= heightFromGround;
        moveRangePosition += Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * distanceFromTarget;
        freeMoveRange.transform.position = moveRangePosition;
        freeMoveRange.transform.up = Vector3.up;

        transform.LookAt(freeMoveRange.transform.position);

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
            case (StateId.STATIONARY):
            {
                Vector3 delta = characterController.GetPositionDelta();
                Vector3 cameraDelta = GetForwardTranslation(delta);
                Vector3 moveRangeDelta = Vector3.Lerp(freeMoveRange.transform.position, target.transform.position, 0.1f);
                transform.position += cameraDelta;
                transform.position += GetVerticalTranslation(moveRangeDelta.y - freeMoveRange.transform.position.y);
                freeMoveRange.transform.position = moveRangeDelta;
                transform.position = Vector3.Lerp(transform.position, FindClosestPointOnRadius(), cameraInertia);
                transform.LookAt(freeMoveRange.transform.position + lookAboveVector);
                break;
            }
            case (StateId.FOLLOW_TARGET):
            {
                Vector3 delta = characterController.GetPositionDelta();
                Vector3 cameraDelta = GetForwardTranslation(delta);
                transform.position += GetVerticalTranslation(delta.y);
                transform.position += cameraDelta;
                freeMoveRange.transform.Translate(delta);
                transform.position = Vector3.Lerp(transform.position, FindClosestPointOnRadius(), cameraInertia);
                transform.LookAt(freeMoveRange.transform.position + lookAboveVector);
                break;
            }
            case (StateId.RETURN_TO_CENTER):
            {
                // return to center is where we "apply the brakes"
                break;
            }
            case (StateId.FREE_LOOK):
            {
                RotateCameraHorizontal(InputBuffer.cameraHorizontal);
                RotateCameraVertical(InputBuffer.cameraVertical);

                Vector3 delta = characterController.GetPositionDelta();
                Vector3 moveRangeDelta = ApproachTarget(freeMoveRange.transform.position + delta);
                transform.position += GetVerticalTranslation(moveRangeDelta.y - freeMoveRange.transform.position.y);
                transform.position += delta;
                freeMoveRange.transform.position += delta;
                freeMoveRange.transform.position = moveRangeDelta;
                transform.position = Vector3.Lerp(transform.position, FindClosestPointOnRadius(), cameraInertia);
                transform.LookAt(freeMoveRange.transform.position + lookAboveVector);
                break;
            }
            case (StateId.WAITING):
            {
                Vector3 delta = characterController.GetPositionDelta();
                Vector3 moveRangeDelta = ApproachTarget(freeMoveRange.transform.position + delta);
                transform.position += GetVerticalTranslation(moveRangeDelta.y - freeMoveRange.transform.position.y);
                transform.position += delta;
                freeMoveRange.transform.position += delta;
                freeMoveRange.transform.position = moveRangeDelta;
                transform.position = Vector3.Lerp(transform.position, FindClosestPointOnRadius(), cameraInertia);
                transform.LookAt(freeMoveRange.transform.position + lookAboveVector);
                break;
            }
        }

    }

    private void Update()
    {
        UpdateStateMachine();

        switch (cameraMach.GetCurrentStateId())
        {
            case (StateId.STATIONARY):
            {
                break;
            }
            case (StateId.FOLLOW_TARGET):
            {
                break;
            }
            case (StateId.RETURN_TO_CENTER):
            {
                cameraMach.CommandMachine(CommandId.STAY_PUT);
                break;
            }
            case (StateId.WAITING):
            {
                timer += Time.deltaTime;
                break;
            }
        }
    }

    private void InitializeStateMachine()
    {
        cameraMach = new StateMachine(StateId.STATIONARY);

        cameraMach.AddState(StateId.FREE_LOOK);
        cameraMach.AddState(StateId.FOLLOW_TARGET);
        cameraMach.AddState(StateId.WAITING);
        cameraMach.AddState(StateId.RETURN_TO_CENTER);

        cameraMach.LinkStates(StateId.STATIONARY, StateId.FREE_LOOK, CommandId.ORBIT);
        cameraMach.LinkStates(StateId.STATIONARY, StateId.FOLLOW_TARGET, CommandId.FOLLOW);

        cameraMach.LinkStates(StateId.FREE_LOOK, StateId.WAITING, CommandId.WAIT);

        cameraMach.LinkStates(StateId.FOLLOW_TARGET, StateId.STATIONARY, CommandId.STAY_PUT);
        cameraMach.LinkStates(StateId.FOLLOW_TARGET, StateId.FREE_LOOK, CommandId.ORBIT);

        cameraMach.LinkStates(StateId.WAITING, StateId.RETURN_TO_CENTER, CommandId.TIME_OUT);
        cameraMach.LinkStates(StateId.WAITING, StateId.FREE_LOOK, CommandId.ORBIT);

        cameraMach.LinkStates(StateId.RETURN_TO_CENTER, StateId.STATIONARY, CommandId.STAY_PUT);
        cameraMach.LinkStates(StateId.RETURN_TO_CENTER, StateId.FREE_LOOK, CommandId.ORBIT);

        cameraMach.AddPauseState();
    }

    private void UpdateStateMachine()
    {
		// pausing should short circuit any updates
		if (cameraMach.GetCurrentStateId() == StateId.PAUSE)
		{
			return;
		}

        if (Utilities.GetCameraStickPosition() != Controls.neutralStickPosition)
        {
            cameraMach.CommandMachine(CommandId.ORBIT);
        }
        else
        {
            cameraMach.CommandMachine(CommandId.WAIT);
        }

        if (timer > resetWaitTime)
        {
            timer = 0;
            cameraMach.CommandMachine(CommandId.TIME_OUT);
        }

        if (rangeController.GetCurrentState() == StateId.TARGET_INSIDE_RANGE)
        {
            if (characterController.GetPositionDelta() == Vector3.zero && Vector3.Distance(transform.position, CalculateRequiredDistance()) > 0.1f
				&& timer > resetWaitTime)
			{
				// cameraMach.CommandMachine(CommandId.RESET);
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

    private Vector3 GetForwardTranslation(Vector3 deltaIn)
    {
        Vector3 forwardTranslation = transform.forward.normalized;
        deltaIn.y = 0;
        forwardTranslation.y = 0;

        return Vector3.Project(deltaIn, forwardTranslation);
    }

    private Vector3 GetVerticalTranslation(float moveRangeHeightDelta)
    {
        Vector3 vertTranslation = characterController.GetVerticalDelta();
        float checkAngle = GetVerticalAngle();

        if (checkAngle >= maxVerticalAngle  && vertTranslation.y < 0)
        {
            return vertTranslation;
        }
        else if (vertTranslation.y > 0)
        {
            return new Vector3(0, moveRangeHeightDelta, 0);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Finds the closest point on the circle around the center of the moveRAnge
    /// </summary>
    /// <returns>The closest point on radius.</returns>
    /// <param name="deltaIn">Delta in.</param>
    private Vector3 FindClosestPointOnRadius()
    {
        Vector3 flatPosition = transform.position;
        Vector3 rangeFlatPosition = freeMoveRange.transform.position;
        float checkAngle = GetVerticalAngle();
        if (checkAngle > maxVerticalAngle)
        {
            // rotate pointOnCircle around freeMoveRange.transform.position, axis'd to transform.right
            flatPosition = Quaternion.AngleAxis(-1 * (checkAngle - maxVerticalAngle), transform.right) * flatPosition;
        }
        else if (checkAngle < -1 * maxVerticalAngle)
        {
            flatPosition = Quaternion.AngleAxis(checkAngle - maxVerticalAngle, transform.right) * flatPosition;
        }

        // C = Center of circle (so position of moveRange,
        // P = position of Camera
        // V = P - C
        Vector3 pointOnCircle = flatPosition - rangeFlatPosition;
        pointOnCircle = pointOnCircle.normalized * distanceFromTarget; // flat radius
        pointOnCircle += rangeFlatPosition;
        pointOnCircle.y = transform.position.y;
        
        return pointOnCircle;
    }

    private Vector3 ApproachTarget(Vector3 position)
    {
        return Vector3.Lerp(position, target.transform.position, cameraInertia * Time.fixedDeltaTime);
    }
    #endregion

    #region CameraRotation
    private void RotateCameraHorizontal(float degree)
    {
        transform.RotateAround(freeMoveRange.transform.position, Vector3.up, degree * cameraMoveSpeed * Time.deltaTime);
    }

    private float GetVerticalAngle()
    {
        Vector3 flatForward = transform.forward;
        flatForward.y = 0;
        float checkAngle = Vector3.SignedAngle(flatForward, transform.forward, transform.right);

        return checkAngle;
    }

    private void RotateCameraVertical(float degree)
    {
        float checkAngle = GetVerticalAngle();
        if (checkAngle < maxVerticalAngle && degree < 0 || checkAngle >  -1 * maxVerticalAngle && degree > 0)
        {
            transform.RotateAround(freeMoveRange.transform.position, transform.right, -1 * degree * cameraMoveSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region StateSaving
    public void Pause()
	{
		currentStateId = cameraMach.GetCurrentStateId();
		cameraMach.CommandMachine(CommandId.PAUSE);
        rangeController.Pause();
	}

	public void Unpause()
	{
		cameraMach.ForceStateChange(currentStateId);
        rangeController.Unpause();
	}

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
	#endregion
}
