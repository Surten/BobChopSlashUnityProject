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
    private float fieldOfViewAngle;
    private float groundDistance = 0.2f; //enemy's height from origin
    private float jumpRange = 1.0f;
    private float staggerProb = 0.0f;
    private float height = 0f;

    private float obstructionHeight = 0f;
    private float obstructionDistance = 0f;
    private Vector3 obstructionPoint;

    protected float attackRadius;
    protected int attackDamage;
    protected float scalingFactor = 1.0f;

    protected int coins;
    protected int exp;
    protected bool isBoss;

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
    private bool isMoving;

    private float staggerTimeMax;
    protected float staggerTime;
    private float despawnTime;
    private float stateChangeTime;
    private float movingMemoryTime;
    private float movingMemoryFrame;
    private float forgetMemoryTime;
    private float forgetMemoryFrame;

    public GameObject FloatingTextPrefab;
    public AudioSource audioSource;
    public List<AudioClip> clips;
    private Collider enemyCollider;

    private LayerMask detectableLayers;
    private bool safeHavenDetected;

    public enum EnemyState 
    { 
        Dead = 0, 
        Idle, 
        Jumping, 
        Staggering, 
        Rotating, 
        Wandering, 
        Walking, 
        Running, 
        Crawling, 
        Climbing, 
        Attacking, 
        Biting, 
        Kicking, 
        Choking, 
        Howling, 
        Explode, 
        Frozen 
    }

    public enum SoundState
    {
    }

    /* Initialization and Updates per Frame */
    protected virtual void Start()
    {
        SetDetectableLayers(~(1 << LayerMask.NameToLayer("Enemy")));
        SetSafeHavenDetected(false);

        SetIsStateChanged(false);
        SetIsStaggering(false);
        SetStaggerTime(0);

        SetIsMoving(false);
        ResetMovingMemoryTime();

        SetRigidBody(GetComponent<Rigidbody>());
        SetCollider(GetComponent<Collider>());
        SetHeight(CalculateHeight());

        if (enemyCollider == null)
        {
            UnityEngine.Debug.LogError("Collider not found on the enemy GameObject.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource not found or assigned!");
            return;
        }
    }

    protected virtual void Update() { SetStateChangeTime(Time.deltaTime); }

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

    public void SetIsBoss(bool val) { isBoss = val;}

    public bool GetIsBoss() { return isBoss; }

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

    public void SetCollider(Collider collider) { enemyCollider = collider; }

    public Collider GetCollider() { return enemyCollider;  }

    public void SetCoins(int val) { coins = val; }

    public int GetCoins() { return coins;}

    public void SetExp(int val){ exp = val; }

    public int GetExp() { return exp; }

    public void SetIsMoving(bool val) { isMoving = val;}

    public bool GetIsMoving() { return isMoving; }

    public void SetMovingMemoryTime(float val) { movingMemoryTime += val; }

    public float GetMovingMemoryTime() { return movingMemoryTime; }

    public void ResetMovingMemoryTime() { movingMemoryTime = 0f; }

    public void SetMovingMemoryFrame(float val) { movingMemoryFrame = val; }

    public float GetMovingMemoryFrame() { return movingMemoryFrame; }

    public void SetForgetMemoryTime(float val) { forgetMemoryTime -= val; }

    public float GetForgetMemoryTime() { return forgetMemoryTime; }

    public void ResetForgetMemoryTime() { forgetMemoryTime = forgetMemoryFrame; }

    public void SetForgetMemoryFrame(float val) { forgetMemoryFrame = val; }

    public float GetForgetMemoryFrame() { return forgetMemoryFrame; }

    public bool HasForgottenPlayer() { return forgetMemoryTime < 0f; }

    public void SetForgetPlayer() { forgetMemoryTime = -1f; }

    public void SetFieldOfViewAngle(float val) { fieldOfViewAngle = val; }

    public float GetFieldOfViewAngle() { return fieldOfViewAngle; }

    public void SetObstructionHeight(float val) { obstructionHeight = val; }

    public float GetObstructionHeight() { return obstructionHeight; }

    public void SetObstructionDistance(float val) { obstructionDistance = val; }
    public float GetObstructionDistance() { return obstructionDistance; }

    public void SetObstructionPoint(Vector3 val) { obstructionPoint = val; }

    public Vector3 GetObstructionPoint() { return obstructionPoint; }

    public void SetHeight(float val) { height = val; }

    public float GetHeight() { return height; }

    public void SetDetectableLayers(LayerMask val) { detectableLayers = val; }

    public LayerMask GetDetectableLayers() { return detectableLayers; }

    public void SetSafeHavenDetected(bool val) { safeHavenDetected = val; }

    public bool GetSafeHavenDetected() { return safeHavenDetected; }

    public void ScaleAttackDamage(float val) { attackDamage = (int)(attackDamage * val); }

    public void SetScalingFactor(float val) { scalingFactor = val; }

    public float GetScalingFactor() { return scalingFactor; }

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

    public float EnemyNPlayerAngle() {
        Vector3 directionToPlayer = target.position - transform.position;
        return Vector3.Angle(transform.forward, directionToPlayer);
    }

    public void RotateToTarget()
    {
        Vector3 v = target.position - transform.position;
        v.y = 0;
        var q = Quaternion.LookRotation(v);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, GetRotateSpeed() * Time.deltaTime);
    }

    public void Transition2Position(float speed)
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    public void RotateNMove2Position(float speed) 
    {
        if (!GetIsMoving())
        {
            RotateToTarget();
            if (EnemyNPlayerAngle() < GetFieldOfViewAngle() * 0.5f) SetIsMoving(true);
        }
        else {
            SetMovingMemoryTime(Time.deltaTime);
            if (GetMovingMemoryTime() > GetMovingMemoryFrame()) {
                SetIsMoving(false);
                ResetMovingMemoryTime();
            }
        }
        Transition2Position(speed);
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

    public IEnumerator LerpObstacle(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
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

    public bool EnemyDetected(float maxDistance) //Needs Improvements
    {
        RaycastHit hit;
        Vector3 enemyPos = transform.position;
        enemyPos.y += height;
        Vector3 directionToPlayer = target.position - enemyPos;

        // Perform a raycast to check for obstacles between the enemy and the player
        if (Physics.Raycast(enemyPos, directionToPlayer, out hit, maxDistance, detectableLayers))
        {
            // Adjust this condition based on your game's logic

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                SetSafeHavenDetected(false);
                UnityEngine.Debug.DrawRay(enemyPos, directionToPlayer * hit.distance, Color.yellow);
                // The player is not obstructed by an obstacle
                return true;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("SafeHaven")) {
                SetSafeHavenDetected(true);
                UnityEngine.Debug.DrawRay(enemyPos, directionToPlayer * hit.distance, Color.blue);
            }
            else
            {
                SetSafeHavenDetected(false);
                UnityEngine.Debug.DrawRay(enemyPos, directionToPlayer * 1000, Color.white);
            }
        }
        return false;
    }

    public bool CheckObstruction(float maxDistance) //Needs Improvements
    {
        RaycastHit hit;
        Vector3 enemyPos = transform.position;
        enemyPos.y += 0.1f;
        Vector3 directionToPlayer = target.position - enemyPos;
        //directionToPlayer.y = enemyPos.y; //Debug

        // Perform a raycast to check for obstacles between the enemy and the player
        if (Physics.Raycast(enemyPos, directionToPlayer, out hit, maxDistance, detectableLayers))
        {
            // Adjust this condition based on your game's logic

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {

                // Retrieve the distance to the hit point
                SetObstructionDistance(hit.distance);
                SetObstructionPoint(hit.point);
                SetObstructionHeight(hit.collider.bounds.size.y);
                UnityEngine.Debug.DrawRay(enemyPos, directionToPlayer * hit.distance, Color.green);
                return true;
            }
        }
        return false;
    }

    protected float CalculateHeight() {
        CapsuleCollider capsuleCollider = (CapsuleCollider)enemyCollider;
        return capsuleCollider.height * 0.7f;
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