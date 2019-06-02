using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryCameraController : MonoBehaviour {

    [SerializeField] private float cameraRotationSpeed;
    [SerializeField] private float cameraDistance;
    [SerializeField] private float maxVerticalAngle;
    [SerializeField] private GameObject targetObject;

    [SerializeField] private TargetType cameraTargetType;

    private Vector3 target;

	// Use this for initialization
	void Start () {
        target = Vector3.zero;

        switch (cameraTargetType)
        {
            case (TargetType.CENTER_OF_LEVEL):
                if (targetObject != null)
                {
                    target = targetObject.transform.position; // the center of the level should always be zero
                }
                break;
            case (TargetType.LEVEL_MOVING):
                // todo: add levels that move, then refactor old camera control targeting a portion of the level instead of thep layer
                break;
            case (TargetType.PLAYER):
                // todo: refactor old camera control here
                break;
        }

        Vector3 cameraPosition = target;
        cameraPosition.z -= cameraDistance;

        transform.position = cameraPosition;
        transform.LookAt(target);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        switch (cameraTargetType)
        {
            case (TargetType.CENTER_OF_LEVEL):
                RotateCameraHorizontal(InputBuffer.cameraHorizontal);
                RotateCameraVertical(InputBuffer.cameraVertical);
                transform.LookAt(target);
                break;
            case (TargetType.LEVEL_MOVING):
                break;
            case (TargetType.PLAYER):
                break;
        }
    }

    private void RotateCameraHorizontal(float degree)
    {
        transform.RotateAround(target, Vector3.up, degree * cameraRotationSpeed * Time.fixedDeltaTime);
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
        if (checkAngle < maxVerticalAngle && degree > 0 || checkAngle > -1 * maxVerticalAngle && degree < 0)
        {
            transform.RotateAround(target, transform.right, degree * cameraRotationSpeed * Time.deltaTime);
        }
    }

    private enum TargetType
    {
        ERROR_TYPE = -1,
        CENTER_OF_LEVEL,
        LEVEL_MOVING,
        PLAYER,
    }
}
