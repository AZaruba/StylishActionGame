using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : MonoBehaviour {

    #region fields
    public static bool attackPressed;
    public static bool attackDown;
    public static bool jumpPressed;
    public static bool jumpDown;
    public static bool interactPressed;
    public static bool interactDown;

    public static bool pausePressed;

    public static float moveHorizontal;
    public static float moveVertical;

    public static float cameraHorizontal;
    public static float cameraVertical;
    #endregion

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 moveInput = new Vector2(horizontalInput, verticalInput);
        float camHorizontalInput = Input.GetAxis("RHorizontal");
        float camVerticalInput = Input.GetAxis("RVertical");
        Vector2 camInput = new Vector2(camHorizontalInput, camVerticalInput);

        attackPressed = Input.GetKeyDown(Controls.Attack);
        attackDown = Input.GetKey(Controls.Attack);
        jumpPressed = Input.GetKeyDown(Controls.Jump);
        jumpDown = Input.GetKey(Controls.Jump);
        interactPressed = Input.GetKeyDown(Controls.Interact);
        interactDown = Input.GetKey(Controls.Interact);

        pausePressed = Input.GetKeyDown(Controls.Pause);

        if (moveInput.magnitude > Controls.deadZone)
        {
            moveHorizontal = horizontalInput;
            moveVertical = verticalInput;
        }
        else
        {
            moveHorizontal = 0.0f;
            moveVertical = 0.0f;
        }
        if (camInput.magnitude > Controls.deadZone)
        {
            cameraHorizontal = camHorizontalInput;
            cameraVertical = camVerticalInput;
        }
        else
        {
            cameraHorizontal = 0.0f;
            cameraVertical = 0.0f;
        }
    }
}
