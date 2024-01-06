using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeHaven : MonoBehaviour
{

    public ParticleSystem particleSystem1;
    public ParticleSystem particleSystem2;
    public CapsuleCollider colliderHaven;
    public Material activeMaterial;
    public Material inactiveMaterial;

    private float blinkInterval = 0.5f; // Time interval between blinks
    private float timeToBlink = 5f; // Time until the Safe Haven disappears
    private Coroutine blinkingCoroutine;

    public void setBlinkInterval(float val) { blinkInterval = val; }

    public float getBlinkInterval() { return blinkInterval; }

    public void setTimeToBlink(float val) { timeToBlink = val; }

    public float getTimeToBlink() { return timeToBlink; }

    public void activateHaven(float maxSafeTime) 
    {
        StartParticlesSystem();
        EnableMeshCollider();
        SetMaterial(activeMaterial);
        Invoke("StartBlinking", maxSafeTime - timeToBlink);
    }

    public void deactivateHaven() 
    {
        StopBlinking();
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
        if (colliderHaven != null)
        {
            colliderHaven.enabled = true;
        }
    }
    void DisableMeshCollider()
    {
        // Disable the Mesh Collider
        if (colliderHaven != null)
        {
            colliderHaven.enabled = false;
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

    /* Blinking Coroutine */

    void StartBlinking()
    {
        if (blinkingCoroutine == null)
        {
            blinkingCoroutine = StartCoroutine(BlinkCoroutine());
        }
    }

    void StopBlinking()
    {
        if (blinkingCoroutine != null)
        {
            StopCoroutine(blinkingCoroutine);
            blinkingCoroutine = null;
        }
    }

    IEnumerator BlinkCoroutine()
    {
        Material[] materials = { activeMaterial, inactiveMaterial };
        int currentMaterialIndex = 0;

        float timer = 0f;
        while (true)
        {
            SetMaterial(materials[currentMaterialIndex]); // Change material
            currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length; // Toggle between 0 and 1
            yield return new WaitForSeconds(blinkInterval);

            if (timer >= timeToBlink)
            {
                SetMaterial(inactiveMaterial); // Ensure it's inactive before disappearing
                break;
            }
            timer += Time.deltaTime;
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        
    }
    */

}
