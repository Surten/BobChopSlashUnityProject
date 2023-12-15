using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSky: MonoBehaviour
{
    public float RotateSpeed = 0.5f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotateSpeed);
    }
}

