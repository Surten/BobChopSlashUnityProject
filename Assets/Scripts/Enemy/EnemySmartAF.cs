using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.CullingGroup;
using System.Net;
using static WavUtility;
using System.Collections.Generic;
using System.Collections.Specialized;

public class EnemySmartAF : MonoBehaviour
{
    private float rotateSpeed;
    private float walkSpeed;
    private float runSpeed;
    private float awarenessAwareRange;
    private float awarenessChaseRange;
    private float awarenessAttackRange;
    private float groundDistance = 0.2f; //enemy's height from origin
    private float jumpRange = 1.0f;
    private float staggerProb = 0.0f;

    private float attackRadius;
    private int attackDamage;

    private EnemyState currentState = EnemyState.Idle;

    public Enemy1ScriptableObject enemyScriptableObject;
    public Transform target;
    public AttackMelee attackMelee;

    private Vector3 previousTargetPosition;

    private Rigidbody rb;
    public Transform groundCheck;
    public LayerMask groundLayerMask;

    public bool isCharging;
    public bool isIdleJumping;
    private bool isGrounded;
    private bool isStaggering;
    private bool isStateChanged;

    public bool canExplode;
    private ParticleSystem explosionEffect;

    private float staggerTimeMax;
    private float staggerTime;
    private float despawnTime;
    private float quickDespawnTime;
    private float stateChangeTime;

    public GameObject FloatingTextPrefab;
    public AudioSource audioSource;
    public List<AudioClip> clips;


    public enum EnemyState{ Idle, Staggering, Jumping, Rotating, Walking, Charging, Attack, Explode, Dead }
    public enum SoundState { Explosion, Detection, Charge }
    //public Dictionary<SoundState, string> soundpath = new Dictionary<SoundState, string>();

    /* Initialization and Updates per Frame */
    private void Start()
    {
        rotateSpeed = enemyScriptableObject.rotateSpeed;
        walkSpeed = enemyScriptableObject.walkSpeed;
        runSpeed = enemyScriptableObject.runSpeed;

        awarenessAwareRange = enemyScriptableObject.awarenessAwareRange;
        awarenessChaseRange = enemyScriptableObject.awarenessChaseRange;
        awarenessAttackRange = enemyScriptableObject.awarenessAttackRange;

        attackMelee.attackRadius = attackRadius = enemyScriptableObject.attackRadius;
        attackMelee.attackDamage = attackDamage = enemyScriptableObject.attackDamage;

        isCharging = enemyScriptableObject.chargeProbability > UnityEngine.Random.value;
        canExplode = (enemyScriptableObject.kamikazeProbability > UnityEngine.Random.value) && isCharging;
        if (canExplode) attackMelee.attackDamage *= 3;
        isIdleJumping = (enemyScriptableObject.idleJumpProbability) > UnityEngine.Random.value;

        isStateChanged = false;

        isStaggering = false;
        staggerProb = enemyScriptableObject.staggerProbability;
        staggerTimeMax = enemyScriptableObject.staggerTime;
        staggerTime = 0;

        despawnTime = enemyScriptableObject.despawnTime;
        quickDespawnTime = enemyScriptableObject.quickDespawnTime;

        previousTargetPosition = target.position;

        stateChangeTime = 0;

        rb = GetComponent<Rigidbody>();
        explosionEffect = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource not found or assigned!");
            return;
        }

/*        clips.Add(WavUtility.ToAudioClip(UnityEngine.Application.dataPath + "/Sounds/huh.wav"));
        clips.Add(WavUtility.ToAudioClip(UnityEngine.Application.dataPath + "/Sounds/Bomb Explosion.wav"));
        clips.Add(WavUtility.ToAudioClip(UnityEngine.Application.dataPath + "/Sounds/Enemy Detected.wav"));*/

        /*
                // Use utility function to add multiple entries
                _addEntries(soundpath, new Dictionary<SoundState, string>
                {
                    {SoundState.Explosion, "Sounds/Bomb Explosion.wav"},
                    {SoundState.Charge, "Sounds/Yaaa.wav"},
                    {SoundState.Detection, "Sounds/Enemy Detected.wav"}
                });*/
    }

    private void Update()
    {
        stateChangeTime += Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Staggering:
                renderTextureColor();
                UpdateStaggerTime();
                break;

            case EnemyState.Jumping:
                Jump();
                break;

            case EnemyState.Rotating:
                RotateToTarget();
                break;

            case EnemyState.Walking:
                renderTextureColor();
                RotateToTarget();
                Transition2Position(walkSpeed);
                break;

            case EnemyState.Charging:
                renderTextureColor();
                if (canExplode) RotateToTarget(25f);
                else RotateToTarget(-25f);
                Transition2Position(runSpeed);
                break;

            case EnemyState.Attack:
                RotateToTarget();
                attackMelee.SwingSword();
                break;

            case EnemyState.Explode:
                RotateToTarget();
                Explode();
                break;

        }

    }

    /* Status Functions */

    public void SetNewTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetEnemyState(EnemyState state)
    {
        if (currentState == state) return;
        if (stateChangeTime > 1f) { 
            isStateChanged = true;
            stateChangeTime = 0;
        }

        currentState = state;
    }

    public bool GetIsStateChanged() { 
        return isStateChanged; 
    
    }

    public void ResetIsStateChanged() 
    {
        isStateChanged = false;
    }

    public EnemyState GetEnemyState()
    {
        return currentState;
    }

    public float GetDespawnTime()
    {
        return despawnTime;
    }

    public void ShowFloatingText(string text, Color textColor, float sizeMult = 1f, bool showflg = true) {
        if (FloatingTextPrefab == null) return;
        if (!showflg) return;
        var go = Instantiate(FloatingTextPrefab, transform.position, target.rotation, transform);
        go.GetComponent<TextMesh>().text = text;
        go.GetComponent<TextMesh>().characterSize *= sizeMult;
        go.GetComponent<TextMesh>().color = textColor;
    }


    public void renderTextureColor() {
        // Ensure the object has a renderer component
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        // Create a new material instance to avoid modifying the shared material
        Material material = renderer.material;
        Material newMaterial = new Material(material);

        if (currentState == EnemyState.Dead) {
            // Set the new color
            newMaterial.color = Color.black; // Dead
        }
        else if (isStaggering)
        {
            // Set the new color
            newMaterial.color = Color.yellow; // Staggering
        }
        else if (canExplode)
        {
            // Set the new color
            newMaterial.color = Color.red; // Rushing to Explode
        }
        else if (isCharging)
        {
            newMaterial.color = Color.blue; // Rushing
        }
        else
        {
            // Set the new color
            newMaterial.color = Color.green; // Walking
        }

        // Assign the new material to the renderer
        renderer.material = newMaterial;
    }


    /* Action Functions */

    private void RotateToTarget(float inclinationAngle = 0)
    {
        Vector3 v = target.position - transform.position;
        v.y = 0;
        var q = Quaternion.LookRotation(v);

        // Introduce a small inclination towards the target
        Quaternion inclination = Quaternion.AngleAxis(inclinationAngle, -transform.right);

        // Combine the original rotation with the inclination
        q *= inclination;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);
        if (isGrounded)
        {

            float jumpVelocity = _calculateJumpVelocity(jumpRange, 90f); //Adquire the initial jump velocity according to the provided height
            
            // Set the rigidbody's velocity directly
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
    }

    private void Transition2Position(float speed)
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }


    public void StaggerCoinFlip()
    {
        if (staggerProb > UnityEngine.Random.value)
        {
            SetStagger();
        }
    }

    private void SetStagger()
    {
        SetEnemyState(EnemyState.Staggering);
        staggerTime = staggerTimeMax;
        isStaggering = true;

    }

    private void UpdateStaggerTime()
    {
        staggerTime -= Time.deltaTime;
        if (staggerTime < 0)
        {
            SetEnemyState(EnemyState.Idle);
            isStaggering = false;
            return;
        }
    }

    private void Explode()
    {
        attackMelee.SwingSword();
        if (GetComponent<ParticleSystem>() != null) // Play explosion particles
        {
            explosionEffect.Play();

        }

        SetEnemyState(EnemyState.Dead); // Change Enemy State to dead
        despawnTime = quickDespawnTime; // Speed up removal from scene

        ShowFloatingText("0", Color.white, 1f, false);
    }

    /* Sound Functions */
    public void LoadWavFile(int clipNum)
    {
        /*        string path = string.Format("{0}/{1}", UnityEngine.Application.dataPath, filename);
                AudioClip audioClip = WavUtility.ToAudioClip(path);
                audioSource.clip = audioClip;
                UnityEngine.Debug.Log(audioSource.clip.length);
                audioSource.Play();*/

        audioSource.clip = clips[clipNum];
        audioSource.Play();
    }

    /* List of Subfunctions (functions that are used as tools for other functions)*/

    // Utility function to add multiple entries to a dictionary
    private static void _addEntries<TKey, TValue>(Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> entries)
    {
        foreach (var entry in entries)
        {
            dictionary[entry.Key] = entry.Value;
        }
    }

    private float _calculateJumpVelocity(float jumpHeight, float angle = 0f)
    {
        // Calculate the Straight Up Jump Velocity: v_0 = sqrt(2 * g * h / sin^2(angle))
        float sinSquared = _calculateSinSquared(angle);
        if (sinSquared < 1e-3) return 0f;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float jumpVelocity = Mathf.Sqrt(2f * gravity * jumpHeight / sinSquared);
        return jumpVelocity;
    }

    private float _calculateSinSquared(float angleInDegrees)
    {
        // Convert angle to radians since Mathf.Sin uses radians
        float angleInRadians = Mathf.Deg2Rad * angleInDegrees;

        // Calculate sin^2(theta)
        return _squared(Mathf.Sin(angleInRadians));
    }

    private float _squared(float value) {
        return value * value;
    }
}
