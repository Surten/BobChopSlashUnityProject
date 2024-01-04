using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Torch : MonoBehaviour
{
    public GameObject flame;
    public Light lightSource;
    public float detectionRange = 10f; // The range within which the player triggers the Torch

    private ParticleSystem fire;
    private Transform player; // Reference to the player's transform
    private bool isOn;

    // Start is called before the first frame update
    void Start()
    {
        fire = flame.GetComponent<ParticleSystem>();

        DisableTorch();

        // Find the player's GameObject and get its transform
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Check if the player was found
        if (player == null)
        {
            Debug.LogError("Player not found. Make sure the player has a tag 'Player'.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is within the detection range
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            // Enable the light source and particle system
            if (!isOn) EnableTorch();
        }
        else
        {
            // Disable the light source and particle system
            if (isOn) DisableTorch();
        }
    }

    void EnableTorch() {
        lightSource.enabled = true;
        fire.Play();
        isOn = true;
    }

    void DisableTorch() {
        lightSource.enabled = false;
        fire.Stop();
        isOn = false;
    }
}
