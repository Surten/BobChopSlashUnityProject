using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 1f;
    public Vector3 Offset = new Vector3(0f, 1.5f, 0f);
    public Vector3 RandomizeIntensity = new Vector3(0.2f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DestroyTime);
        
        transform.localPosition += Offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
        Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y),
        Random.Range(-RandomizeIntensity.z, RandomizeIntensity.z));
    }
}
