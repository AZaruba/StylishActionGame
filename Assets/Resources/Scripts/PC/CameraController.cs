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
        rBody.MovePosition(transform.position + manager.SharePlayerTranslation());
        rBody.transform.LookAt(playerPos);

        if (Mathf.Abs(Input.GetAxis("RHorizontal")) > Controls.deadZone)
        {
            Debug.Log("aaaa");
            rBody.MovePosition(Quaternion.AngleAxis(rotationSpeed * Input.GetAxis("RHorizontal") * Time.deltaTime * -1, Vector3.up) * (rBody.transform.position - playerPos) + playerPos);
        }
	}
}
