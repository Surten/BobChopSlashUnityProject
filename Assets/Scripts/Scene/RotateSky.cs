using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky: MonoBehaviour
{
    public float RotateSpeed = 0.5f;
    public Material newSkyboxMaterial; // Skybox material in the Inspector
    public Light directionalLight; // Assign your directional light in the Inspector
    // Start is called before the first frame update

    // Update is called once per frame
    private void OnEnable()
    {
        // Clear the previous skybox by setting it to null
        RenderSettings.skybox = null;
        RenderSettings.sun = null;

        // Change the skybox material
        RenderSettings.skybox = newSkyboxMaterial;
        RenderSettings.skybox.SetFloat("_Rotation", 0);
        RenderSettings.sun = directionalLight;
    }

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
    }
}

