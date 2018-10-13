using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    List<IEnemy> enemies;
    CharacterMovementController playerCharacter;

    private Vector3 playerPosition;
    private Vector3 playerTrans;
    private bool playerMove;

    public Transform sphereEnemy;

	// Use this for initialization
	void Start () {
        playerCharacter = FindObjectOfType<CharacterMovementController>(); // skipping Singleton pattern for multiplayer possibilities!
        Instantiate(sphereEnemy, new Vector3(3, 1, 3), Quaternion.identity);
        Instantiate(sphereEnemy, new Vector3(-3, 1, 3), Quaternion.identity); // disabling while we fix slopes

        playerPosition = playerCharacter.SendPosition();
        playerMove = false;
        playerTrans = Vector3.zero;

        StationarySphere[] spheres = FindObjectsOfType<StationarySphere>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        playerPosition = playerCharacter.SendPosition();
        playerTrans = playerCharacter.SendHorizontalTranslation();
        playerMove = playerCharacter.SendIsMoving();
	}

    public Vector3 SharePlayerPosition()
    {
        return playerPosition;
    }
    public Vector3 SharePlayerTranslation()
    {
        return playerTrans;
    }
    public bool SharePlayerMoving()
    {
        return playerMove;
    }
}
