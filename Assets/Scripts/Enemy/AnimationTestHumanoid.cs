using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class AnimationTestHumanoid : MonoBehaviour
{

    public Animator anim;
    public enum EnemyState { Idle, Staggering, Rotating, Walking, Running, Climbing, Attack, Kicking, Choking, Dead };
    private EnemyState currentState;
    bool playStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        currentState = EnemyState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    void UpdateState()
    {

        if (IsPlaying()) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                UnityEngine.Debug.Log("Transition to Stagger!");
                currentState = EnemyState.Staggering;
                break;

            case EnemyState.Staggering:
                UnityEngine.Debug.Log("Transition to Rotate!");
                currentState = EnemyState.Rotating;
                break;

            case EnemyState.Rotating:
                UnityEngine.Debug.Log("Transition to Walk!");
                currentState = EnemyState.Walking;
                break;

            case EnemyState.Walking:
                UnityEngine.Debug.Log("Transition to Run!");
                currentState = EnemyState.Running;
                break;

            case EnemyState.Running:
                UnityEngine.Debug.Log("Transition to Climbing!");
                currentState = EnemyState.Climbing;
                break;

            case EnemyState.Climbing:
                UnityEngine.Debug.Log("Transition to Climbing!");
                currentState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                UnityEngine.Debug.Log("Transition to Kicking!");
                currentState = EnemyState.Kicking;
                break;

            case EnemyState.Kicking:
                UnityEngine.Debug.Log("Transition to Choking!");
                currentState = EnemyState.Choking;
                break;

            case EnemyState.Choking:
                UnityEngine.Debug.Log("Transition to Dead!");
                currentState = EnemyState.Dead;
                break;

            case EnemyState.Dead:
                UnityEngine.Debug.Log("Transition to Idle!");
                currentState = EnemyState.Idle;
                break;

            default:
                currentState = EnemyState.Idle;
                break;
        }

        animate();
    }

    bool IsPlaying()
    {
        // Check if the specified animation is currently playing
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        return currentState.normalizedTime < 1.0f;
    }

    void PlayAnimation(string stateName)
    {
        // Play the specified animation
        anim.Play(stateName, 0, 0f);
    }

    void animate() 
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                PlayAnimation("Idle");
                break;

            case EnemyState.Staggering:
                PlayAnimation("Reaction Hit");
                break;

            case EnemyState.Rotating:
                PlayAnimation("Rotate");
                break;

            case EnemyState.Walking:
                PlayAnimation("Walk");
                break;

            case EnemyState.Running:
                PlayAnimation("Run");
                break;

            case EnemyState.Climbing:
                PlayAnimation("Climb");
                break;

            case EnemyState.Attack:
                PlayAnimation("Attack");
                break;

            case EnemyState.Kicking:
                PlayAnimation("Kick");
                break;

            case EnemyState.Choking:
                PlayAnimation("Choke");
                break;

            case EnemyState.Dead:
                PlayAnimation("Death");
                break;

            default:
                currentState = EnemyState.Idle;
                break;

        }
    }
}
