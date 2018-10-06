﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour {

    public float moveSpeed;
    public float deadZone;
    // private int collisionMask = 1 << 10;
    // private float currentSpeed;

    private Camera mainCam;
    private CharacterCombatController combat;
    public Rigidbody rBody;


    // Use this for initialization
    void Start()
    {
        Physics.IgnoreLayerCollision(11, 10); // ignore collisions between player and weapon
        // gravity = gameObject.GetComponent<GravityController>(); // coming soon
        combat = gameObject.GetComponent<CharacterCombatController>();

        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        if (!combat.IsAttacking())
        {
            if (Mathf.Abs(Input.GetAxis("Vertical")) > deadZone || Mathf.Abs(Input.GetAxis("Horizontal")) > deadZone)
            {
                rBody.MovePosition(transform.position + WalkInput()); // need to figure out "sticking"
                // transform.position += WalkInput();
            }
        }


    }

    private Vector3 WalkInput()
    {
        Vector3 direction = new Vector3();
        Vector3 camDirection = GetCameraOrientation();

        float rotationDegree = Mathf.Atan2(Input.GetAxis("Vertical"), -1 * Input.GetAxis("Horizontal"));
        direction.x = -1 * camDirection.z * Mathf.Cos(rotationDegree) + camDirection.x * Mathf.Sin(rotationDegree);
        direction.z = camDirection.z * Mathf.Sin(rotationDegree) + camDirection.x * Mathf.Cos(rotationDegree);

        direction.Normalize();
        camDirection.Normalize();

        transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        direction *= moveSpeed * Time.deltaTime;

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
}
