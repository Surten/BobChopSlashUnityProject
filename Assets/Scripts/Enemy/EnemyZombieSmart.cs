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

public class EnemyZombieSmart : EnemySmart
{
    private bool isBiting;

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
        Attack = 4,
        Biting = 4,
        Dead = 0,
        Frozen = 0,
    }

    private Animator anim;
    private AnimatorStateInfo animState;

    /* Initialization and Updates per Frame */
    protected override void Start()
    {
        base.Start();
        SetRotateSpeed(enemyScriptableObject.rotateSpeed);
        SetWalkSpeed(enemyScriptableObject.walkSpeed);
        SetRunSpeed(enemyScriptableObject.runSpeed);

        SetAwarenessAwareRange(enemyScriptableObject.awarenessAwareRange);
        SetAwarenessChaseRange(enemyScriptableObject.awarenessChaseRange);
        SetAwarenessAttackRange(enemyScriptableObject.awarenessAttackRange);

        attackMelee = GetComponent<ZombieAttack>();
        SetAtackRadius(enemyScriptableObject.attackRadius);
        SetAttackDamage(enemyScriptableObject.attackDamage);
        isBiting = Prob2Bool(enemyScriptableObject.biteProbability);

        isCharging = Prob2Bool(enemyScriptableObject.chargeProbability);

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
        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Staggering:
                if (animState.IsName("Reaction Hit")) UpdateStaggerTime();
                break;

            case EnemyState.Rotating:
                if (animState.IsName("Rotate")) RotateToTarget();
                break;

            case EnemyState.Walking:
                if (animState.IsName("Walk"))
                {
                    RotateToTarget();
                    Transition2Position(GetWalkSpeed());
                }
                break;

            case EnemyState.Running:
                if (animState.IsName("Run"))
                {
                    RotateToTarget();
                    Transition2Position(GetRunSpeed());
                }
                break;

            case EnemyState.Attack:
                if (animState.IsName("Attack"))
                {
                    RotateToTarget();
                    attackMelee.SwingArm();

                    if (!IsPlaying("Attack")) PlayAnimation("Attack");
                    
                }
                break;

            case EnemyState.Biting:
                if (animState.IsName("Bite"))
                {
                    RotateToTarget();
                    attackMelee.Bite();

                    if (!IsPlaying("Bite")) PlayAnimation("Bite");

                }
                break;
            default:
                break;

        }

        if (GetIsStateChanged() | (animState.normalizedTime >= 1.0f)) {
            animate();
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
        ResetTriggers();
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

        float targetDistance = (target.position - transform.position).magnitude; // Check for the distance between player and enemy
        if (targetDistance > enemyScriptableObject.awarenessAwareRange) // No movement
        {
            if (isStateChanged) ResetIsStateChanged();
            SetEnemyState(EnemyState.Idle);
            return;
        }
        else if (targetDistance > enemyScriptableObject.awarenessChaseRange) // Rotate towards player
        {
            if (isStateChanged) ResetIsStateChanged();
            SetEnemyState(EnemyState.Rotating);
            return;
        }
        else if (targetDistance > enemyScriptableObject.awarenessAttackRange) // Chase Player
        {
            if (isStateChanged) ResetIsStateChanged();
            if (isCharging) SetEnemyState(EnemyState.Running);
            else SetEnemyState(EnemyState.Walking);
            return;
        }
        else // Attack player
        {
            if (isStateChanged) ResetIsStateChanged();
            if (isBiting) SetEnemyState(EnemyState.Biting);
            else SetEnemyState(EnemyState.Attack);
            return;
        }
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

    public void animate()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                anim.SetTrigger("Idle");
                LoadWavFile(Sound2Int(SoundState.Idle));
                break;

            case EnemyState.Staggering:
                anim.SetTrigger("Reaction Hit");
                anim.SetTrigger("Idle");
                LoadWavFile(Sound2Int(SoundState.Staggering));
                break;

            case EnemyState.Rotating:
                anim.SetTrigger("Rotate");
                anim.SetTrigger("Idle");
                LoadWavFile(Sound2Int(SoundState.Rotating));
                break;

            case EnemyState.Walking:
                anim.SetTrigger("Walk");
                LoadWavFile(Sound2Int(SoundState.Walking));
                break;

            case EnemyState.Running:
                anim.SetTrigger("Idle");
                anim.SetTrigger("Walk");
                anim.SetTrigger("Run");
                LoadWavFile(Sound2Int(SoundState.Running));
                break;

            case EnemyState.Attack:
                anim.SetTrigger("Idle");
                anim.SetTrigger("Attack");
                LoadWavFile(Sound2Int(SoundState.Attack));
                break;

            case EnemyState.Biting:
                anim.SetTrigger("Idle");
                anim.SetTrigger("Bite");
                LoadWavFile(Sound2Int(SoundState.Biting));
                break;

            case EnemyState.Dead:
                anim.SetTrigger("Death");
                LoadWavFile(Sound2Int(SoundState.Dead));
                break;

            case EnemyState.Frozen:
                anim.SetTrigger("Frozen");
                anim.SetTrigger("Idle");
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
