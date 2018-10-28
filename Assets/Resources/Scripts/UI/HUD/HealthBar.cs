using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public Material healthBarShader;

    private void Start()
    {
        healthBarShader.SetFloat("_Cutoff", 0);
    }

    public void UpdateHealthBar(float valueIn)
    {
        Debug.Log("setting alpha to " + valueIn);
        healthBarShader.SetFloat("_Cutoff", valueIn);
    }
}
