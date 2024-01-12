﻿using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.CullingGroup;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;

public class EnemyZombieSmart : EnemySmart
{
    private bool isBiting;
    private bool isCrawling;

    public Enemy2ScriptableObject enemyScriptableObject;
    protected ZombieAttack attackMelee;

    public new enum SoundState
    {
        Idle = 1,
        Alert = 0,
        Staggering = 2,
        Jumping = 0,
        Rotating = 0,
        Walking = 3,
        Running = 3,
        Crawling = 3,
        Attack = 4,
        Biting = 4,
        Dead = 0,
        Frozen = 0,
    }

    public Animator anim;
    private AnimatorStateInfo animState;

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

        attackMelee = GetComponent<ZombieAttack>();
        SetAtackRadius(enemyScriptableObject.attackRadius);
        SetAttackDamage(enemyScriptableObject.attackDamage);
        ScaleAttackDamage(GetScalingFactor());
        isBiting = Prob2Bool(enemyScriptableObject.biteProbability);

        isCharging = Prob2Bool(enemyScriptableObject.chargeProbability);
        isCrawling = isCharging && Prob2Bool(enemyScriptableObject.crawlProbability);

        SetMovingMemoryFrame(enemyScriptableObject.movingMemoryFrame);
        SetForgetMemoryFrame(enemyScriptableObject.forgottenMemoryFrame);
        SetFieldOfViewAngle(enemyScriptableObject.fieldOfViewAngle);

        SetStaggerProb(enemyScriptableObject.staggerProbability);
        SetStaggerTimeMax(enemyScriptableObject.staggerTime);

        SetDespawnTime(enemyScriptableObject.despawnTime);

        SetPreviousTargetPos(target.position);

        ResetStateChangeTime();

        anim = GetComponentInChildren<Animator>();
        
    }

    protected override void Update()
    {
        base.Update();
        
        animState = anim.GetCurrentAnimatorStateInfo(0);
        
        if (GetIsStateChanged())
        {
            animate();
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Staggering:
                if (!animState.IsName("Reaction Hit") && !animState.IsName("Death")) animate();
                UpdateStaggerTime();
                break;

            case EnemyState.Rotating://Must modify Rotate animation so it animates correct direction in animation
                if (!animState.IsName("Rotate")) animate();
                if (!IsPlaying("Rotate")) PlayAnimation("Rotate"); 
                RotateToTarget();
                break;

            case EnemyState.Walking:
                if (!animState.IsName("Walk")) animate();
                RotateNMove2Position(GetWalkSpeed());
                break;

            case EnemyState.Running:
                if (!animState.IsName("Run")) animate();
                RotateNMove2Position(GetRunSpeed());
                break;

            case EnemyState.Crawling:
                if (!animState.IsName("Crawl")) animate();
                RotateNMove2Position(GetRunSpeed());
                break;

            case EnemyState.Climbing:
                if (!animState.IsName("Climb")) animate();
                Vector3 pos = GetObstructionPoint();
                pos.y = GetObstructionHeight();
                StartCoroutine(LerpObstacle(pos, 1f));
                break;

            case EnemyState.Attacking:
                RotateToTarget();
                attackMelee.SwingArm();
                break;

            case EnemyState.Biting:
                RotateToTarget();
                attackMelee.Bite();
                break;

            default:
                break;
        }
    }
    bool IsPlaying(string stateName)
    {
        // Check if the specified animation is currently playing
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        return currentState.IsName(stateName) && currentState.normalizedTime < 1.0f;
    }

    void PlayAnimation(string stateName)
    {
        // Play the specified animation
        anim.Play(stateName, 0, 0f);
    }

    public void SetAtackRadius(float val) { attackMelee.attackRadius = attackRadius = val; }

    public float GetAtackRadius() { return attackRadius; }

    public void SetAttackDamage(int val) { attackMelee.attackDamage = attackDamage = val; }

    public int GetAttackDamage() { return attackDamage; }

    public override void SetEnemyState(EnemyState state)
    {
        base.SetEnemyState(state);
    }

    public new EnemyState GetEnemyState()
    {
        return currentState;
    }

    public override void SetStagger()
    {
        SetEnemyState(EnemyState.Staggering);
        LoadWavFile(Sound2Int(SoundState.Staggering));
        base.SetStagger();
        attackMelee.StopAttack();
    }

    public override void UpdateStaggerTime()
    {
        staggerTime -= Time.deltaTime;
        if (staggerTime < 0)
        {
            SetEnemyState(EnemyState.Idle);
            SetIsStaggering(false);
            return;
        }
    }

    /* Action Functions */
    public void Behaviour()
    {
        bool isStateChanged = GetIsStateChanged();

        if (GetEnemyState() == EnemyState.Staggering)
        { // If the enemy is staggering, do nothing;
            if (isStateChanged) ResetIsStateChanged();
            return;
        }

        bool detected = EnemyDetected(GetAwarenessAwareRange());
        float targetDistance = (target.position - transform.position).magnitude; // Check for the distance between player and enemy

        if (attackMelee.isAttacking) { return; }

        if (detected) ResetForgetMemoryTime();
        else if (GetSafeHavenDetected()) SetForgetPlayer();
        else SetForgetMemoryTime(Time.deltaTime);

        if ((!detected & HasForgottenPlayer()) | (targetDistance > GetAwarenessAwareRange())) // No movement
        {
            if (isStateChanged) ResetIsStateChanged();
            SetEnemyState(EnemyState.Idle);
            return;
        }
        else if (targetDistance > GetAwarenessChaseRange()) // Rotate towards player
        {
            if (isStateChanged) ResetIsStateChanged();
            SetEnemyState(EnemyState.Rotating);
            return;
        }
        else if (targetDistance > GetAwarenessAttackRange()) // Chase Player
        {
            bool obstructed = CheckObstruction(GetAwarenessChaseRange());

            if (obstructed && GetObstructionDistance() < 0.6f)
            {
                if (isStateChanged) ResetIsStateChanged();
                SetEnemyState(EnemyState.Climbing);
            }
            else {
                if (isStateChanged) ResetIsStateChanged();
                if (isCrawling) SetEnemyState(EnemyState.Crawling);
                else if (isCharging) SetEnemyState(EnemyState.Running);
                else SetEnemyState(EnemyState.Walking);
            }
            return;
        }
        else // Attack player
        {
            if (isStateChanged) ResetIsStateChanged();
            if (isBiting) SetEnemyState(EnemyState.Biting);
            else SetEnemyState(EnemyState.Attacking);
            return;
        }
    }

    /* Animation Functions */
    public override void animate()
    {
        anim.speed = 1.0f;
        switch (currentState)
        {
            case EnemyState.Idle:
                PlayAnimation("Idle");
                LoadWavFile(Sound2Int(SoundState.Idle));
                break;

            case EnemyState.Staggering:
                PlayAnimation("Reaction Hit");
                LoadWavFile(Sound2Int(SoundState.Staggering));
                break;

            case EnemyState.Rotating:
                PlayAnimation("Rotate");
                LoadWavFile(Sound2Int(SoundState.Rotating));
                break;

            case EnemyState.Walking:
                anim.speed = GetWalkSpeed();
                PlayAnimation("Walk");
                LoadWavFile(Sound2Int(SoundState.Walking));
                break;

            case EnemyState.Running:
                anim.speed = GetRunSpeed()/2;
                PlayAnimation("Run");
                LoadWavFile(Sound2Int(SoundState.Running));
                break;

            case EnemyState.Crawling:
                anim.speed = GetRunSpeed()/2;
                PlayAnimation("Crawl");
                LoadWavFile(Sound2Int(SoundState.Crawling));
                break;

            case EnemyState.Climbing:
                PlayAnimation("Climb");
                break;

            case EnemyState.Attacking:
                PlayAnimation("Attack");
                LoadWavFile(Sound2Int(SoundState.Attack));
                break;

            case EnemyState.Biting:
                PlayAnimation("Bite");
                LoadWavFile(Sound2Int(SoundState.Biting));
                break;

            case EnemyState.Dead:
                PlayAnimation("Death");
                LoadWavFile(Sound2Int(SoundState.Dead));
                break;

            case EnemyState.Frozen:
                //PlayAnimation("Frozen");
                LoadWavFile(Sound2Int(SoundState.Frozen));
                break;

            default:
                currentState = EnemyState.Idle;
                ResetStateChangeTime();
                break;

        }
    }

    /* List of Subfunctions (functions that are used as tools for other functions)*/
    private int Sound2Int(SoundState sound) { return (int)sound; }

}
