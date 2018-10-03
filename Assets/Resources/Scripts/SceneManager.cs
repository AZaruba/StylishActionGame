using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    List<IEnemy> enemies;
    CharacterMovementController playerCharacter;

    private Vector3 playerPosition;

    public Transform sphereEnemy;

	// Use this for initialization
	void Start () {
        playerCharacter = FindObjectOfType<CharacterMovementController>(); // skipping Singleton pattern for multiplayer possibilities!
        Instantiate(sphereEnemy, new Vector3(3, 1, 3), Quaternion.identity);
        Instantiate(sphereEnemy, new Vector3(-3, 1, 3), Quaternion.identity);

        playerPosition = playerCharacter.SendPosition();

        StationarySphere[] spheres = FindObjectsOfType<StationarySphere>();
	}
	
	// Update is called once per frame
	void Update () {
        playerPosition = playerCharacter.SendPosition();
	}

    public Vector3 SharePlayerPosition()
    {
        return playerPosition;
    }
}
