using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.CullingGroup;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;

public class EnemySmart : MonoBehaviour
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

    protected float attackRadius;
    protected int attackDamage;

    protected int coins;

    protected EnemyState currentState = EnemyState.Idle;

    public Transform target;
    //public AttackMelee attackMelee;

    protected Vector3 previousTargetPosition;

    protected Rigidbody rb;
    public Transform groundCheck;
    public LayerMask groundLayerMask;

    public bool isCharging;
    private bool isGrounded;
    private bool isStaggering;
    private bool isStateChanged;

    private float staggerTimeMax;
    protected float staggerTime;
    private float despawnTime;
    private float stateChangeTime;

    public GameObject FloatingTextPrefab;
    public AudioSource audioSource;
    public List<AudioClip> clips;


    //public enum EnemyState { Dead=0, Idle, Staggering, Rotating, Walking, Charging, Attack, Frozen }
    public enum EnemyState { Dead = 0, Idle, Jumping, Staggering, Rotating, Walking, Running, Attack, Biting, Explode, Frozen }

    public enum SoundState
    {
    }

    /* Initialization and Updates per Frame */
    protected virtual void Start()
    {
        //UnityEngine.Debug.Log("Base Start");

        SetIsStateChanged(false);
        SetIsStaggering(false);
        SetStaggerTime(0);

        SetRigidBody(GetComponent<Rigidbody>());
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource not found or assigned!");
            return;
        }
    }

    protected virtual void Update()
    {
        //UnityEngine.Debug.Log("Base Update");
        SetStateChangeTime(Time.deltaTime);
    }
    /* Get, Set, Reset Functions*/
    public void SetRotateSpeed(float val) { rotateSpeed = val;}

    public float GetRotateSpeed() { return rotateSpeed;}

    public void SetWalkSpeed(float val) {  walkSpeed = val;}

    public float GetWalkSpeed() {  return walkSpeed;}

    public void SetRunSpeed(float val) { runSpeed = val; }

    public float GetRunSpeed() { return runSpeed; }

    public void SetAwarenessAwareRange(float val) { awarenessAwareRange = val; }

    public float GetAwarenessAwareRange() {  return awarenessAwareRange;}

    public void SetAwarenessChaseRange(float val) { awarenessChaseRange = val;  }

    public float GetAwarenessChaseRange() {  return awarenessChaseRange; }

    public void SetAwarenessAttackRange(float val) { awarenessAttackRange = val; }

    public float GetAwarenessAttackRange() {  return awarenessAttackRange; }

    public void SetIsStateChanged(bool val) { isStateChanged = val; }

    public bool GetIsStateChanged() { return isStateChanged; }

    public void ResetIsStateChanged(){ isStateChanged = false; }
    public void SetIsStaggering(bool val) { isStaggering = val; }

    public bool GetIsStaggering() {  return isStaggering; }

    public void SetStaggerProb(float val) {  staggerProb = val; }

    public float GetStaggerProb() {  return staggerProb; }

    public void SetStaggerTimeMax(float val) { staggerTimeMax = val; }

    public float GetStaggerTimeMax() { return staggerTimeMax; }

    public void SetStaggerTime(float val) { staggerTime = val; }

    public float GetStaggerTime() { return staggerTime; }

    public void SetDespawnTime(float val) { despawnTime = val; }

    public float GetDespawnTime() {  return despawnTime; }

    public void SetPreviousTargetPos(Vector3 pos) { previousTargetPosition =  pos; }

    public Vector3 GetPreviousTargetPos() {  return previousTargetPosition; }

    public void SetStateChangeTime(float val) { stateChangeTime += val; }

    public float GetStateChangeTime() { return stateChangeTime; }

    public void ResetStateChangeTime() { stateChangeTime = 0f; }

    public void SetRigidBody(Rigidbody body) { rb = body; }

    public Rigidbody GetRigidbody() { return rb; }

    public void SetCoins(int val) { coins = val; }

    public int GetCoins() { return coins;}

    /* Status Functions */

    public void SetNewTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public virtual void SetEnemyState(EnemyState state)
    {
        if (currentState == state) return;
        if (GetStateChangeTime() > 1f)
        {
            SetIsStateChanged(true);
            ResetStateChangeTime();
        }

        currentState = state;
    }

    public EnemyState GetEnemyState()
    {
        return currentState;
    }

    public void ShowFloatingText(string text, Color textColor, float sizeMult = 1f, bool showflg = true)
    {
        if (FloatingTextPrefab == null) return;
        if (!showflg) return;
        var go = Instantiate(FloatingTextPrefab, transform.position, target.rotation, transform);
        go.GetComponent<TextMesh>().text = text;
        go.GetComponent<TextMesh>().characterSize *= sizeMult;
        go.GetComponent<TextMesh>().color = textColor;
    }

    public virtual void animate() {
        return;
    }

    /* Action Functions */

    public void RotateToTarget()
    {
        Vector3 v = target.position - transform.position;
        v.y = 0;
        var q = Quaternion.LookRotation(v);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, GetRotateSpeed() * Time.deltaTime);
    }

    public void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);
        if (isGrounded)
        {

            float jumpVelocity = _calculateJumpVelocity(jumpRange, 90f); //Adquire the initial jump velocity according to the provided height

            // Set the rigidbody's velocity directly
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
    }

    public void Transition2Position(float speed)
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    public bool Prob2Bool(float prob) { return (prob > UnityEngine.Random.value);}

    public void StaggerCoinFlip()
    {
        if (staggerProb > UnityEngine.Random.value) { SetStagger(); }
    }

    public virtual void SetStagger()
    {
        staggerTime = staggerTimeMax;
        isStaggering = true;

    }

    public virtual void UpdateStaggerTime()
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
    public void LoadWavFile(int clipNum)
    {
        if (clipNum == 0) return;
        audioSource.clip = clips[clipNum];
        audioSource.Play();
    }

    /* List of Subfunctions (functions that are used as tools for other functions)*/

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

    private float _squared(float value)
    {
        return value * value;
    }
}