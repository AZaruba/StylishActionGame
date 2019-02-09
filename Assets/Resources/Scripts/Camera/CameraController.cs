using System.Collections;
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
        InitializeCameraPosition();

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
        Vector3 oldFMPosition = freeMoveRange.transform.position + lookAboveVector;
        
        switch (cameraMach.GetCurrentStateId())
        {
            case (StateId.STATIONARY):
            {
                Vector3 delta = characterController.GetPositionDelta();
                Vector3 cameraDelta = GetForwardTranslation(delta);
                Vector3 moveRangeDelta = Vector3.Lerp(freeMoveRange.transform.position, target.transform.position, 0.1f);

                transform.position += cameraDelta;
                
                freeMoveRange.transform.position = moveRangeDelta;
                transform.position = Vector3.Lerp(transform.position, FindClosestFlatPointOnRadius(), cameraInertia * Time.deltaTime * 2);
                transform.position += GetVerticalTranslation(moveRangeDelta.y);
                transform.LookAt(Vector3.Lerp(oldFMPosition, freeMoveRange.transform.position + lookAboveVector, cameraInertia));
                break;
            }
            case (StateId.FOLLOW_TARGET):
            {
                // we need to fix the case in which the player outpaces the moveRange
                Vector3 delta = characterController.GetPositionDelta();
                Vector3 cameraDelta = GetForwardTranslation(delta);

                transform.position += cameraDelta;

                freeMoveRange.transform.position += delta;
                transform.position = Vector3.Lerp(transform.position, FindClosestPointOnRadius(), cameraInertia);
                transform.position += GetVerticalTranslation(delta.y);
                transform.LookAt(Vector3.Lerp(oldFMPosition, freeMoveRange.transform.position + lookAboveVector, cameraInertia));
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
                Vector3 cameraDelta = delta;
                cameraDelta.y = 0;
                Vector3 moveRangeDelta = ApproachTarget(freeMoveRange.transform.position + delta);

                transform.position += cameraDelta;
                
                freeMoveRange.transform.position = moveRangeDelta;
                transform.position = Vector3.Lerp(transform.position, FindClosestPointOnRadius(), cameraInertia);
                transform.position += GetVerticalTranslation(moveRangeDelta.y);
                transform.LookAt(Vector3.Lerp(oldFMPosition, freeMoveRange.transform.position + lookAboveVector, cameraInertia));
                break;
            }
            case (StateId.WAITING):
            {
                Vector3 delta = characterController.GetPositionDelta();
                Vector3 cameraDelta = delta;
                cameraDelta.y = 0;
                Vector3 moveRangeDelta = ApproachTarget(freeMoveRange.transform.position + delta);

                transform.position += cameraDelta;
                freeMoveRange.transform.position = moveRangeDelta;
                transform.position = Vector3.Lerp(transform.position, FindClosestPointOnRadius(), cameraInertia);
                transform.position += GetVerticalTranslation(moveRangeDelta.y);
                transform.LookAt(Vector3.Lerp(oldFMPosition, freeMoveRange.transform.position + lookAboveVector, cameraInertia));
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

    private void InitializeCameraPosition()
    {
        Vector3 cameraPosition = target.transform.position;
        cameraPosition -= Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * distanceFromTarget;
        cameraPosition.y += heightFromGround;
        transform.position = cameraPosition;
        transform.up = Vector3.up;
        lookAboveVector = new Vector3(0, lookHeightAboveTarget, 0);

        Vector3 moveRangePosition = transform.position;
        moveRangePosition.y -= heightFromGround;
        moveRangePosition += Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * distanceFromTarget;
        freeMoveRange.transform.position = moveRangePosition;
        freeMoveRange.transform.up = Vector3.up;

        transform.LookAt(freeMoveRange.transform.position);
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
            cameraMach.CommandMachine(CommandId.STAY_PUT);
        }
        else if (rangeController.GetCurrentState() == StateId.TARGET_OUTSIDE_RANGE)
        {
            cameraMach.CommandMachine(CommandId.FOLLOW);
        }
    }

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
            return vertTranslation; // new Vector3(0, moveRangeHeightDelta, 0);
        }
        else if (vertTranslation.y > 0)
        {
            return vertTranslation; // new Vector3(0, moveRangeHeightDelta, 0);
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
            flatPosition = Quaternion.AngleAxis(maxVerticalAngle - checkAngle, transform.right) * (flatPosition - rangeFlatPosition) + flatPosition;
        }
        else if (checkAngle < -1 * maxVerticalAngle)
        {
            flatPosition = Quaternion.AngleAxis(checkAngle + maxVerticalAngle, transform.right) * (flatPosition - rangeFlatPosition) + flatPosition;
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

    /// <summary>
    /// Finds the closest point on the circle parallel to the player
    /// </summary>
    /// <returns></returns>
    private Vector3 FindClosestFlatPointOnRadius()
    {
        Vector3 flatPosition = transform.position;
        Vector3 rangeFlatPosition = freeMoveRange.transform.position;
        float checkAngle = GetVerticalAngle();

        if (checkAngle > maxVerticalAngle)
        {
            // rotate pointOnCircle around freeMoveRange.transform.position, axis'd to transform.right
            flatPosition = Quaternion.AngleAxis(maxVerticalAngle - checkAngle, transform.right) * flatPosition;
        }
        else if (checkAngle < -1 * maxVerticalAngle)
        {
            flatPosition = Quaternion.AngleAxis(checkAngle + maxVerticalAngle, transform.right) * flatPosition;
        }

        // C = Center of circle (so position of moveRange,
        // P = position of Camera
        // V = P - C
        flatPosition.y = rangeFlatPosition.y + heightFromGround;
        Vector3 pointOnCircle = flatPosition - rangeFlatPosition;
        pointOnCircle = pointOnCircle.normalized * distanceFromTarget; // flat radius
        pointOnCircle += rangeFlatPosition;

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
