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

public class EnemyCapsuleSmart : EnemySmart
{
    public Enemy1ScriptableObject enemyScriptableObject;
    protected AttackMelee attackMelee;

    public bool isIdleJumping;

    public bool canExplode;
    private ParticleSystem explosionEffect;

    private float quickDespawnTime;

    public new enum SoundState 
    { 
        Idle = 1,
        Alert = 2, 
        Staggering = 3, 
        Jumping = 0, 
        Rotating = 0, 
        Walking = 0, 
        Running = 4, 
        Attack = 0, 
        Explode = 5, 
        Dead = 6, 
        Frozen = 3
    }

    /* Initialization and Updates per Frame */
    protected override void Start()
    {
        base.Start();

        SetCoins(enemyScriptableObject.coinsDropOnDeath);
        SetExp(enemyScriptableObject.expDropOnDeath);

        SetIsBoss(enemyScriptableObject.isBoss);

        SetRotateSpeed(enemyScriptableObject.rotateSpeed);
        SetWalkSpeed(enemyScriptableObject.walkSpeed);
        SetRunSpeed(enemyScriptableObject.runSpeed);

        SetAwarenessAwareRange(enemyScriptableObject.awarenessAwareRange);
        SetAwarenessChaseRange(enemyScriptableObject.awarenessChaseRange);
        SetAwarenessAttackRange(enemyScriptableObject.awarenessAttackRange);

        attackMelee = GetComponent<AttackMelee>();
        SetAtackRadius(enemyScriptableObject.attackRadius);
        SetAttackDamage(enemyScriptableObject.attackDamage);
        ScaleAttackDamage(GetScalingFactor());

        isCharging = Prob2Bool(enemyScriptableObject.chargeProbability);
        canExplode = Prob2Bool(enemyScriptableObject.kamikazeProbability) && isCharging;
        if (canExplode) attackMelee.attackDamage *= 3;
        isIdleJumping = Prob2Bool(enemyScriptableObject.idleJumpProbability);

        SetMovingMemoryFrame(enemyScriptableObject.movingMemoryFrame);
        SetForgetMemoryFrame(enemyScriptableObject.forgottenMemoryFrame);
        SetFieldOfViewAngle(enemyScriptableObject.fieldOfViewAngle);

        SetStaggerProb(enemyScriptableObject.staggerProbability);
        SetStaggerTimeMax(enemyScriptableObject.staggerTime);

        SetDespawnTime(enemyScriptableObject.despawnTime);
        quickDespawnTime = enemyScriptableObject.quickDespawnTime;

        SetPreviousTargetPos(target.position);

        ResetStateChangeTime();

        explosionEffect = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    protected override void Update()
    {
        base.Update();

        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Staggering:
                animate();
                UpdateStaggerTime();
                break;

            case EnemyState.Jumping:
                Jump();
                break;

            case EnemyState.Rotating:
                RotateToTarget();
                break;

            case EnemyState.Walking:
                animate();
                RotateToTarget();
                Transition2Position(GetWalkSpeed());
                break;

            case EnemyState.Running:
                animate();
                if (canExplode) RotateToTarget(25f);
                else RotateToTarget(-25f);
                Transition2Position(GetRunSpeed());
                break;

            case EnemyState.Attacking:
                RotateToTarget();
                attackMelee.MeleeAttackLight();
                break;

            case EnemyState.Explode:
                RotateToTarget();
                Explode();
                break;

        }

    }

    public void SetAtackRadius(float val) { attackMelee.attackRadius = attackRadius = val; }

    public float GetAtackRadius() { return attackRadius; }

    public void SetAttackDamage(int val) { attackMelee.attackDamage = attackDamage = val; }

    public int GetAttackDamage() { return attackDamage; }
    public override void animate() {
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
        else if (GetIsStaggering())
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
    public void Behaviour()
    {
        bool isStateChanged = GetIsStateChanged();
        float textSizeMult = 2f;
        Color textColor = Color.black;
        bool showflg = false;

        if (currentState == EnemyState.Staggering)
        { // If the enemy is staggering, do nothing;
            if (isStateChanged) ResetIsStateChanged();
            return;
        }

        bool detected = EnemyDetected(GetAwarenessAwareRange());
        float targetDistance = (target.position - transform.position).magnitude; // Check for the distance between player and enemy

        if (detected) ResetForgetMemoryTime();
        else SetForgetMemoryTime(Time.deltaTime);

        if ((!detected & HasForgottenPlayer()) | (targetDistance > GetAwarenessAwareRange())) // No movement
        {
            if (isIdleJumping)
            {
                if (isStateChanged)
                {
                    ShowFloatingText("♪(┌・。・)┌", textColor, textSizeMult * 4f, showflg);
                    ResetIsStateChanged();
                }
                SetEnemyState(EnemyState.Jumping);
            }
            else
            {
                if (isStateChanged)
                {
                    ShowFloatingText("ヽ(•‿•)ノ", textColor, textSizeMult * 4f, showflg);
                    ResetIsStateChanged();
                }
                SetEnemyState(EnemyState.Idle);
            }
            return;
        }
        else if (targetDistance > GetAwarenessChaseRange()) // Rotate towards player
        {
            if (isStateChanged)
            {
                ShowFloatingText("(☉_☉)", textColor, textSizeMult * 2f, showflg);
                LoadWavFile(Sound2Int(SoundState.Alert));
                ResetIsStateChanged();
            }
            SetEnemyState(EnemyState.Rotating);
            return;
        }
        else if (targetDistance > GetAwarenessAttackRange()) // Chase Player
        {
            if (isStateChanged)
            {
                ShowFloatingText("ヽ(ಠ_ಠ)ノ", textColor, textSizeMult, showflg);
                if (isCharging) LoadWavFile(Sound2Int(SoundState.Running));
                ResetIsStateChanged();
            }
            if (isCharging)
            {
                SetEnemyState(EnemyState.Running);
            }
            else
            {
                SetEnemyState(EnemyState.Walking);
            }

            return;
        }
        else // Attack player
        {
            if (isStateChanged)
            {
                ResetIsStateChanged();
            }

            if (canExplode)
            {
                SetEnemyState(EnemyState.Explode);
                LoadWavFile(0);
            }
            else
            {
                SetEnemyState(EnemyState.Attacking);
            }

            return;
        }
    }

    public void RotateToTarget(float inclinationAngle = 0)
    {
        Vector3 v = target.position - transform.position;
        v.y = 0;
        var q = Quaternion.LookRotation(v);

        // Introduce a small inclination towards the target
        Quaternion inclination = Quaternion.AngleAxis(inclinationAngle, -transform.right);

        // Combine the original rotation with the inclination
        q *= inclination;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, GetRotateSpeed() * Time.deltaTime);
    }

    public override void SetStagger()
    {
        SetEnemyState(EnemyState.Staggering);
        LoadWavFile(Sound2Int(SoundState.Staggering));
        base.SetStagger();
    }

    public void Explode()
    {
        attackMelee.ImmediateDamage();
        if (GetComponent<ParticleSystem>() != null) // Play explosion particles
        {
            explosionEffect.Play();
            LoadWavFile(Sound2Int(SoundState.Explode));

        }

        SetEnemyState(EnemyState.Dead); // Change Enemy State to dead
        SetDespawnTime(quickDespawnTime); // Speed up removal from scene

        ShowFloatingText("0", Color.white, 1f, false);
    }

    /* List of Subfunctions (functions that are used as tools for other functions)*/
    public int Sound2Int(SoundState sound) { return (int)sound; }
}
