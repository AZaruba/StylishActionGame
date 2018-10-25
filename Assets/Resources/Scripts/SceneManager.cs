﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    List<IEnemy> enemies;
    CharacterMovementController playerCharacter;
    CharacterHealth playerHealth;

    public Transform sphereEnemy;
    public Transform pcSpawn;

    private bool paused;

	// Use this for initialization
	void Start () {
        playerCharacter = FindObjectOfType<CharacterMovementController>(); // skipping Singleton pattern for multiplayer possibilities!
        playerHealth = FindObjectOfType<CharacterHealth>();
        Instantiate(sphereEnemy, new Vector3(3, 1, 3), Quaternion.identity);
        Instantiate(sphereEnemy, new Vector3(-3, 1, 3), Quaternion.identity); // disabling while we fix slopes

        enemies = new List<IEnemy>();
        StationarySphere[] spheres = FindObjectsOfType<StationarySphere>();
        for (int x = 0; x < spheres.Length; x++)
        {
            enemies.Add(spheres[x]);
        }

        paused = false;

        SetPlayerControls(); // this might need to go in some sort of Options class
	}

    void SetPlayerControls()
    {
        Controls.SetAttack(KeyCode.JoystickButton2);
        Controls.SetJump(KeyCode.JoystickButton0);
        Controls.SetInteract(KeyCode.JoystickButton1);
        // Controls.SetSpecial(KeyCode.JoystickButton3);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
            TogglePause();
	}

    IEnumerator Pause()
    {
        paused = true;
        Time.timeScale = 0;
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                paused = false;
                Time.timeScale = 1;
                yield break;
            }
        }
    }

    public void TogglePause()
    {
        if (paused)
        {
            Time.timeScale = 1;
            paused = false;
        }
        else
        {
            Time.timeScale = 0;
            paused = true;
        }
    }

    public Vector3 SharePlayerPosition()
    {
        return playerCharacter.SendPosition();
    }
    public Vector3 SharePlayerTranslation()
    {
        return playerCharacter.SendHorizontalTranslation();
    }
    public bool SharePlayerMoving()
    {
        return playerCharacter.SendIsMoving();
    }

    public void SendAttack(int damage)
    {
        playerHealth.Damage(damage);
    }

    public void OnDefeat()
    {
        for (int x = 0; x < enemies.Count; x++)
        {
            enemies[x].OnPlayerDefeated();
        }
    }
}
