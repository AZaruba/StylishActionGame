using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelection : MonoBehaviour {

    public UnityEngine.UI.Text[] texts;
    public float deadZone;

    private int currentIndex;
    private bool menuReady; // checks if analog stick has been released

	// Use this for initialization
	void Start () {
        currentIndex = 0;
        menuReady = true;
        UpdatePointerPosition();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Vertical") < deadZone*-1 && menuReady)
        {
            currentIndex++;
            if (currentIndex >= texts.Length)
            {
                currentIndex = texts.Length - 1;
            }
            UpdatePointerPosition();
            menuReady = false;
        }
        else if (Input.GetAxis("Vertical") > deadZone && menuReady)
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }
            UpdatePointerPosition();
            menuReady = false;
        }
        else if (!menuReady && Mathf.Abs(Input.GetAxis("Vertical")) < deadZone)
        {
            menuReady = true;
        }

        if (Input.GetKeyDown(Controls.Confirm))
        {
            switch(currentIndex)
            {
                case 0: // start game
                    SceneManager.LoadScene("SceneOne", LoadSceneMode.Single);
                    break;
                case 1: // activate credits
                    break;
                case 2: // exit game
                    Application.Quit();
                    break;
            }
        }
	}

    private void UpdatePointerPosition()
    {
        UnityEngine.UI.Text adjText = texts[currentIndex];
        Vector3 newPos = adjText.transform.position;
        newPos.x -= 20;
        transform.position = newPos;
    }

    private void StartGame()
    {

    }
}
