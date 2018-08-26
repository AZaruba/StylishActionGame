using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatController : MonoBehaviour {

    private bool isAttacking;
    private bool hasLinked;
    CharacterEnum constants;

	// Use this for initialization
	void Start () {
        constants = gameObject.GetComponent<CharacterEnum>();

        isAttacking = false;
        hasLinked = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(CharacterEnum.Controls.AttackOne))
            StartCoroutine(Attack());


	}

    public IEnumerator Attack()
    {
        isAttacking = true;
        float time = CharacterEnum.AttackTimes.BasicAttackTime;
        LoadAttackTexture(); // TODO: REMOVE

        while (time > 0)
        {
            time -= Time.deltaTime;

            // if the time remaining in the animation reaches the link value, start the link
            /* DISABLING COMBOS until ONE attack works
            if (time <= CharacterEnum.AttackTimes.AttackLinkTime && !hasLinked)
            {
                StartCoroutine(LinkCombo(CharacterEnum.AttackType.FollowupPunch));
            }
            */
            yield return null;
        }
        if (!hasLinked)
            isAttacking = false;

        ResetTexture(); // TODO: REMOVE
        yield return null;
    }

    public IEnumerator FollowupAttack()
    {
        isAttacking = false;
        yield return null;
    }

    public IEnumerator LinkCombo(CharacterEnum.AttackType attackType)
    {
        float time = CharacterEnum.AttackTimes.AttackLinkTime;
        hasLinked = true;

        while (time > 0)
        {
            time -= Time.deltaTime;

            if (Input.GetKeyDown(CharacterEnum.Controls.AttackOne))
            {
                StartCoroutine(FollowupAttack());
            }
            yield return null;
        }
        yield return null;
    }

    /* DEBUG FUNCTIONS
     * The following functions are useful for development purposes, but will be removed
     * as they are replaced with permanent solutions.
     */

    private void LoadAttackTexture()
    {
        Texture2D texture;
        texture = (Texture2D)Resources.Load("VisualAssets/Characters/PC/Textures/testCharAttack");

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }

    private void ResetTexture()
    {
        Texture2D texture;
        texture = (Texture2D)Resources.Load("VisualAssets/Characters/PC/Textures/testChar");

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }
}
