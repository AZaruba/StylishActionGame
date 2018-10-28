using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public Material healthBarShader;
    public float alphaMin;
    public float alphaMax;

    private void Start()
    {
        healthBarShader.SetFloat("_Cutoff", alphaMin);
    }

    public void UpdateHealthBar(float valueIn)
    {
        // valueIn is now a percentage, so we need to put it in the range of alphaMin to alphaMax

        float newAlpha = (alphaMax - alphaMin) * valueIn + alphaMin;
        healthBarShader.SetFloat("_Cutoff", newAlpha);
    }
}
