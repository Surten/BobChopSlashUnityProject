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

public class EnemySmartAF2 : MonoBehaviour
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
    private bool isBiting;

    private EnemyState currentState = EnemyState.Idle;

    public Enemy2ScriptableObject enemyScriptableObject;
    public Transform target;
    private ZombieAttack attackMelee;

    private Vector3 previousTargetPosition;

    private Rigidbody rb;
    public Transform groundCheck;
    public LayerMask groundLayerMask;

    public bool isCharging;
    private bool isGrounded;
    private bool isStaggering;
    private bool isStateChanged;

    private float staggerTimeMax;
    private float staggerTime;
    private float despawnTime;
    private float stateChangeTime;

    public GameObject FloatingTextPrefab;
    public AudioSource audioSource;
    public List<AudioClip> clips;

    public enum EnemyState{ Idle, Jumping, Staggering, Rotating, Walking, Running, Attack, Biting, Dead, Frozen}
    public enum SoundState
    {
        Idle = 1,
        Alert = 0,
        Staggering = 2,
        Jumping = 0,
        Rotating = 0,
        Walking = 3,
        Running = 3,
        Attack = 4,
        Biting = 4,
        Dead = 0,
        Frozen = 0,
    }

    private Animator anim;

    /* Initialization and Updates per Frame */
    private void Start()
    {
        rotateSpeed = enemyScriptableObject.rotateSpeed;
        walkSpeed = enemyScriptableObject.walkSpeed;
        runSpeed = enemyScriptableObject.runSpeed;

        awarenessAwareRange = enemyScriptableObject.awarenessAwareRange;
        awarenessChaseRange = enemyScriptableObject.awarenessChaseRange;
        awarenessAttackRange = enemyScriptableObject.awarenessAttackRange;

        attackMelee = GetComponent<ZombieAttack>();
        attackMelee.attackRadius = attackRadius = enemyScriptableObject.attackRadius;
        attackMelee.attackDamage = attackDamage = enemyScriptableObject.attackDamage;
        isBiting = enemyScriptableObject.biteProbability > UnityEngine.Random.value;

        isCharging = enemyScriptableObject.chargeProbability > UnityEngine.Random.value;

        isStateChanged = false;

        isStaggering = false;
        staggerProb = enemyScriptableObject.staggerProbability;
        staggerTimeMax = enemyScriptableObject.staggerTime;
        staggerTime = 0;

        despawnTime = enemyScriptableObject.despawnTime;

        previousTargetPosition = target.position;

        stateChangeTime = 0;

        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource not found or assigned!");
            return;
        }
        
    }

    private void Update()
    {
        stateChangeTime += Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Staggering:
                UpdateStaggerTime();
                break;

            case EnemyState.Rotating:
                RotateToTarget();
                break;

            case EnemyState.Walking:
                RotateToTarget();
                anim.SetTrigger("Walk");
                Transition2Position(walkSpeed);
                break;

            case EnemyState.Running:
                RotateToTarget();
                anim.SetTrigger("Run");
                Transition2Position(runSpeed);
                break;

            case EnemyState.Attack:
                RotateToTarget();
                if (isBiting) attackMelee.Bite();
                else attackMelee.SwingArm();
                break;

            default:
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
        if (stateChangeTime > 0f) { 
            isStateChanged = true;
            stateChangeTime = 0;
        }

        animate();
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

    /* Sound Functions */
    public void LoadWavFile(SoundState state)
    {
        int clipNum = (int)state;
        if (clipNum == 0) return;
        audioSource.clip = clips[clipNum];
        audioSource.Play();
    }

    /* Animation Functions */
    private void ResetTriggers()
    {
        anim.ResetTrigger("Idle");
        anim.ResetTrigger("Reaction Hit");
        anim.ResetTrigger("Rotate");
        anim.ResetTrigger("Walk");
        anim.ResetTrigger("Run");
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Bite");
        anim.ResetTrigger("Death");
    }

    void animate()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                anim.SetTrigger("Idle");
                LoadWavFile(SoundState.Idle);
                break;

            case EnemyState.Staggering:
                anim.SetTrigger("Reaction Hit");
                LoadWavFile(SoundState.Staggering);
                break;

            case EnemyState.Rotating:
                anim.SetTrigger("Rotate");
                LoadWavFile(SoundState.Rotating);
                break;

            case EnemyState.Walking:
                anim.SetTrigger("Walk");
                LoadWavFile(SoundState.Walking);
                break;

            case EnemyState.Running:
                anim.SetTrigger("Walk");
                anim.SetTrigger("Run");
                LoadWavFile(SoundState.Running);
                break;

            case EnemyState.Attack:
                anim.SetTrigger("Idle");
                anim.SetTrigger("Attack");
                LoadWavFile(SoundState.Attack);
                break;

            case EnemyState.Biting:
                anim.SetTrigger("Idle");
                anim.SetTrigger("Bite");
                LoadWavFile(SoundState.Biting);
                break;

            case EnemyState.Dead:
                anim.SetTrigger("Death");
                LoadWavFile(SoundState.Dead);
                break;

            case EnemyState.Frozen:
                anim.SetTrigger("Frozen");
                LoadWavFile(SoundState.Frozen);
                break;

            default:
                currentState = EnemyState.Idle;
                stateChangeTime = 0;
                break;

        }
    }

    /* List of Subfunctions (functions that are used as tools for other functions)*/
}
