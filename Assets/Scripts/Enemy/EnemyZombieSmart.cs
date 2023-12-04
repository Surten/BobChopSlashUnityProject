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
                Transition2Position(GetWalkSpeed());
                break;

            case EnemyState.Running:
                RotateToTarget();
                anim.SetTrigger("Run");
                Transition2Position(GetRunSpeed());
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
    public void SetAtackRadius(float val) { attackMelee.attackRadius = attackRadius = val; }

    public float GetAtackRadius() { return attackRadius; }

    public void SetAttackDamage(int val) { attackMelee.attackDamage = attackDamage = val; }

    public int GetAttackDamage() { return attackDamage; }

    public override void SetEnemyState(EnemyState state)
    {
        base.SetEnemyState(state);

        animate();
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
                LoadWavFile(Sound2Int(SoundState.Idle));
                break;

            case EnemyState.Staggering:
                anim.SetTrigger("Reaction Hit");
                LoadWavFile(Sound2Int(SoundState.Staggering));
                break;

            case EnemyState.Rotating:
                anim.SetTrigger("Rotate");
                LoadWavFile(Sound2Int(SoundState.Rotating));
                break;

            case EnemyState.Walking:
                anim.SetTrigger("Walk");
                LoadWavFile(Sound2Int(SoundState.Walking));
                break;

            case EnemyState.Running:
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
