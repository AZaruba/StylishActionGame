using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public LevelManager manager;
    public GameObject playerCharacter;
    public Rigidbody rBody;

    public float rotationSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (playerCharacter == null)
        {
            return;
        }

        Vector3 playerPos = manager.SharePlayerPosition();
        rBody.transform.LookAt(playerPos);

        if (Mathf.Abs(Input.GetAxis("RHorizontal")) > Controls.deadZone)
        {
            float cameraSpeed = rotationSpeed * Input.GetAxis("RHorizontal");
            rBody.transform.Translate(Vector3.right * Time.fixedDeltaTime * cameraSpeed);
        }
        rBody.MovePosition(transform.position + manager.SharePlayerTranslation());
    }
}
