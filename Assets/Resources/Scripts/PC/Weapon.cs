using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    private bool isAttacking;
    private bool hasLinked;
    List<CharacterEnum.Attack> attacks;
    CharacterEnum.Attack currentAttack;

    public float debugAttackDistance;
    private Vector3 debugOriginalPosition;

	// Use this for initialization
	void Start () {

        debugOriginalPosition = new Vector3(0.5f,0,0); // PRE ANIMATION DEBUGGING

        isAttacking = false;
        hasLinked = false;

        attacks = new List<CharacterEnum.Attack>();
        currentAttack = null;

        InitializeAttack(0.75f, 0.25f, 10, CharacterEnum.Controls.AttackOne);
        InitializeAttack(1.0f, 0.25f, 15, CharacterEnum.Controls.AttackOne);
        attacks[0].LinkAttacks(attacks[1]);
    }
	
	// Update is called once per frame
	public void StartAttack () {

        // for each potential combo start, we want a StartCoroutine here
        if (Input.GetKeyDown(attacks[0].inputKey) && !isAttacking)
        {
            StartCoroutine(Attack(attacks[0]));
            StartCoroutine(Animate(attacks[0].attackTime));
        }
	}

    private void OnTriggerEnter(Collider collision)
    {
        // this suffices while we still do not have the player taking damage implemented
        if (!isAttacking)
        {
            return;
        }
        
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
        if (enemy == null)
            return;

        enemy.GetHit(currentAttack.damage);
    }

    private void InitializeAttack(float aTime, float lTime, int aDamage, KeyCode key)
    {
        attacks.Add(new CharacterEnum.Attack(aTime, lTime, aDamage, key));
    }

    public bool IsAttacking()
    {
        return this.isAttacking;
    }

    /* COROUTINES
     * Because attacks occur while the user performs some other input, we want them to go in their own coroutines.
     */
    public IEnumerator Attack(CharacterEnum.Attack attack)
    {
        currentAttack = attack; // set attack BEFORE in case coroutine does not set isAttacking
        isAttacking = true;
        float time = attack.attackTime;
        LoadAttackTexture(); // TODO: REMOVE

        while (time > 0)
        {
            time -= Time.deltaTime;

            // if the time remaining in the animation reaches the link value, start the link
            // DISABLING COMBOS until ONE attack works
            if (time <= attack.linkTime && !hasLinked)
            {
                LoadLinkTexture();
                StartCoroutine(LinkCombo(attack.nextAttack, attack.linkTime));
                yield break;
            }

            yield return null;
        }

        isAttacking = false;
        currentAttack = null; // reset attack AFTER in case coroutine does not go past isAttacking
        ResetTexture(); // TODO: REMOVE
        yield return null;
    }

    public IEnumerator LinkCombo(CharacterEnum.Attack nextAttack, float linkTime)
    {
        // short circuit: if we cannot follow up the current attack, just reset to one
        if (nextAttack == null)
        {
            isAttacking = false;
            ResetTexture();
            yield break;
        }

        float time = linkTime;
        hasLinked = true;

        while (time > 0)
        {
            time -= Time.deltaTime;

            if (Input.GetKeyDown(CharacterEnum.Controls.AttackOne))
            {
                hasLinked = false;
                StartCoroutine(Attack(nextAttack));
                yield break;
            }
            yield return null;
        }

        isAttacking = false;
        hasLinked = false;
        ResetTexture();
        yield return null;
    }

    /* DEBUG FUNCTIONS
     * The following functions are useful for development purposes, but will be removed
     * as they are replaced with permanent solutions.
     */

    public IEnumerator Animate(float animTime)
    {
        float time = 0;

        while (time < animTime)
        {

            time += Time.deltaTime;

            if (time > animTime / 2)
            {
                transform.Translate(0, 0, -1 * debugAttackDistance * Time.deltaTime);
            }
            else
            {
                transform.Translate(0, 0, debugAttackDistance * Time.deltaTime);
            }
            yield return null;
        }

        transform.position = transform.parent.position + debugOriginalPosition;
        yield return null;
    }

    private void LoadAttackTexture()
    {
        Texture2D texture;
        texture = (Texture2D)Resources.Load("VisualAssets/Characters/PC/Textures/testCharAttack");

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }

    private void LoadLinkTexture()
    {
        Texture2D texture;
        texture = (Texture2D)Resources.Load("VisualAssets/Characters/PC/Textures/testCharHit");

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }

    private void ResetTexture()
    {
        Texture2D texture;
        texture = (Texture2D)Resources.Load("VisualAssets/Characters/PC/Textures/testChar");

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
    }
}
