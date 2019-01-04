using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float heightFromGround;
    [SerializeField] private float distanceFromTarget;
    [SerializeField] private float pitchRotation;
    [SerializeField] private float horizontalOffset;

    [SerializeField] private float maxVerticalAngle;

    [SerializeField] private GameObject target;
    [SerializeField] private GameObject freeMoveRange;
    [SerializeField] private CameraMoveRangeController rangeController;

    private StateMachine cameraMach;

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
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (rangeController.GetCurrentState() == StateId.IDLE)
        {

        }
        else if (rangeController.GetCurrentState() == StateId.COLLIDING)
        {

        }
	}

    private void InitializeStateMachine()
    {
        cameraMach = new StateMachine(StateId.FREE_LOOK);

        cameraMach.AddState(StateId.FOLLOW_TARGET);

        cameraMach.LinkStates(StateId.FOLLOW_TARGET, StateId.FREE_LOOK, CommandId.FOLLOW);
        cameraMach.LinkStates(StateId.FREE_LOOK, StateId.FOLLOW_TARGET, CommandId.RESET);
    }
}
