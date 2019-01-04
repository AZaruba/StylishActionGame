using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    private bool hasLinked;

    public float debugAttackDistance;
    private Vector3 debugOriginalPosition;
    private Quaternion debugOriginalRotation;

    // Use this for initialization
    void Start() {

        debugOriginalPosition = transform.localPosition;
        debugOriginalRotation = transform.localRotation;

        hasLinked = false;
    }

    // Update is called once per frame
    public void StartAttack() {

    }

    private void OnTriggerEnter(Collider collision)
    {
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
        if (enemy == null)
            return;
    }
    /* DEBUG FUNCTIONS
     * The following functions are useful for development purposes, but will be removed
     * as they are replaced with permanent solutions.
     */

    public IEnumerator Animate(float animTime)
    {
        float time = 0;

        while (time < animTime && !hasLinked) // when animations are implemented, simply cancel the old anim instead of this not statement
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
        transform.rotation = debugOriginalRotation * transform.parent.rotation;
        yield return null;
    }

    // a simple differentiator between the two attacks we are currently working on
    public IEnumerator AnimateTwo(float animTime)
    {
        float time = 0;

        transform.position = transform.parent.position + debugOriginalPosition;
        transform.rotation = debugOriginalRotation * transform.parent.rotation;

        while (time < animTime)
        {

            time += Time.deltaTime;
            transform.Rotate(Vector3.forward, Time.deltaTime * 180, Space.Self);

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
        transform.rotation = debugOriginalRotation * transform.parent.rotation;
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
