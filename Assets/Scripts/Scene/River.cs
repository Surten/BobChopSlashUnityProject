using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class River : MonoBehaviour
{

    public float speedX = 0.1f;
    public float speedY = 0.1f;
    private float curX;
    private float curY;

    // Use this for initialization
    void Start()
    {
        curX = GetComponent<Renderer>().material.mainTextureOffset.x;
        curY = GetComponent<Renderer>().material.mainTextureOffset.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        curX += Time.deltaTime * speedX;
        curY += Time.deltaTime * speedY;
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(curX, curY));
    }

    // OnCollisionStay is called when an object stays in contact with the Collider
    private void OnTriggerStay(Collider other)
    {
        // Check if the object has a Rigidbody
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate the push direction based on the river's movement
            Vector3 pushDirection = new Vector3(speedX*100f, 0f, speedY*100f);

            // Apply the push force to the object's Rigidbody
            rb.AddForce(pushDirection, ForceMode.Force);
        }
    }
}
