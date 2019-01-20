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
        float camHorizontalInput = Input.GetAxis("RHorizontal");
        float camVerticalInput = Input.GetAxis("RVertical");

        attackPressed = Input.GetKeyDown(Controls.Attack);
        attackDown = Input.GetKey(Controls.Attack);
        jumpPressed = Input.GetKeyDown(Controls.Jump);
        jumpDown = Input.GetKey(Controls.Jump);
        interactPressed = Input.GetKeyDown(Controls.Interact);
        interactDown = Input.GetKey(Controls.Interact);

        pausePressed = Input.GetKeyDown(Controls.Pause);

        if (Mathf.Abs(horizontalInput) > Controls.deadZone)
            moveHorizontal = horizontalInput;
        else
            moveHorizontal = 0.0f; // return a value well outside of the range of Atan2

        if (Mathf.Abs(verticalInput) > Controls.deadZone)
            moveVertical = verticalInput;
        else
            moveVertical = 0.0f; // return a value well outside of the range of Atan2

        if (Mathf.Abs(camHorizontalInput) > Controls.deadZone)
            cameraHorizontal = camHorizontalInput;
        else
            cameraHorizontal = 0.0f; // return a value well outside of the range of Atan2

        if (Mathf.Abs(camVerticalInput) > Controls.deadZone)
            cameraVertical = camVerticalInput;
        else
            cameraVertical = 0.0f; // return a value well outside of the range of Atan2
    }
}
