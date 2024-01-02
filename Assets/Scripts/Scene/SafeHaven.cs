using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeHaven : MonoBehaviour
{

    public ParticleSystem particleSystem1;
    public ParticleSystem particleSystem2;
    public MeshCollider meshCollider;
    public Material activeMaterial;
    public Material inactiveMaterial;

    void Start()
    {
        //activateHaven();

        //Invoke("deactivateHaven", 10f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void activateHaven() {
        StartParticlesSystem();
        EnableMeshCollider();
        SetMaterial(activeMaterial);
    }

    public void deactivateHaven() {
        StopParticleSystems();
        DisableMeshCollider();
        SetMaterial(inactiveMaterial);
    }

    /* Render */
    void SetMaterial(Material newMaterial)
    {
        // Check if a new material is assigned
        if (newMaterial != null)
        {
            // Assign the new material to the mesh renderer
            Renderer meshRenderer = GetComponent<Renderer>();
            meshRenderer.material = newMaterial;

        }
        else
        {
            Debug.LogError("Please assign a material in the Inspector.");
        }
    }

    /* Mesh Collider */
    void EnableMeshCollider()
    {
        // Enable the Mesh Collider
        if (meshCollider != null)
        {
            meshCollider.enabled = true;
        }
    }
    void DisableMeshCollider()
    {
        // Disable the Mesh Collider
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }
    }



    /* Particle Systems */

    void StartParticlesSystem() {
        // Start the first particle system immediately
        StartFirstParticleSystem();

        // Start the second particle system with a 1-second delay
        Invoke("StartSecondParticleSystem", 1f);
    }

    void StartFirstParticleSystem()
    {
        // Check if particleSystem1 is not null and is stopped, then play it
        if (particleSystem1 != null && !particleSystem1.isPlaying)
        {
            particleSystem1.Play();
        }
    }

    void StartSecondParticleSystem()
    {
        // Check if particleSystem2 is not null and is stopped, then play it
        if (particleSystem2 != null && !particleSystem2.isPlaying)
        {
            particleSystem2.Play();
        }
    }

    void StopParticleSystems()
    {
        // Check if particleSystem1 is not null and is playing, then stop it
        if (particleSystem1 != null && particleSystem1.isPlaying)
        {
            particleSystem1.Stop();
        }

        // Check if particleSystem2 is not null and is playing, then stop it
        if (particleSystem2 != null && particleSystem2.isPlaying)
        {
            particleSystem2.Stop();
        }
    }


}
