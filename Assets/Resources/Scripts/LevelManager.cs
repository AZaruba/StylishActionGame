using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public Transform sphereEnemy;
    public Transform pcSpawn;

    private bool paused;

	// Use this for initialization
	void Start () {
        paused = false;

        Controls.SetDefaultControls();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(Controls.Pause))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit(); // safety, allows the game to be ended at any time
	}

    public void TogglePause()
    {
        if (paused)
        {
			UnpauseAllEntities();
            paused = false;
        }
        else
        {
			PauseAllEntities();
            paused = true;
        }
    }

	private void UnpauseAllEntities()
	{
        // we know there will be only one character controller and one camera controller
        CharacterMasterController charController = (CharacterMasterController)FindObjectOfType(typeof(CharacterMasterController));
        CameraController camController = (CameraController)FindObjectOfType(typeof(CameraController));

        charController.Unpause();
        camController.Unpause();
	}

	private void PauseAllEntities()
	{
        CharacterMasterController charController = (CharacterMasterController)FindObjectOfType(typeof(CharacterMasterController));
        CameraController camController = (CameraController)FindObjectOfType(typeof(CameraController));

        charController.Pause();
        camController.Pause();
	}
}
