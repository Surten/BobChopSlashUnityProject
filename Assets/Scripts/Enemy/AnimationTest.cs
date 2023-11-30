using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class AnimationTest : MonoBehaviour
{

    public Animator anim;
    public enum EnemyState { Idle, Staggering, Rotating, Walking, Running, Attack, Biting, Dead };
    private EnemyState currentState;
    private float stateChangeTime;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        currentState = EnemyState.Idle;
        UnityEngine.Debug.Log("Hello, Unity!");
    }

    // Update is called once per frame
    void Update()
    {
        stateChangeTime += Time.deltaTime;
        SetState();
        ResetTriggers();
        animate();
    }

    public void SetState()
    {
        if (stateChangeTime < 5f) return;
        stateChangeTime = 0;
        UpdateState();
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                currentState = EnemyState.Staggering;
                break;

            case EnemyState.Staggering:
                currentState = EnemyState.Rotating;
                break;

            case EnemyState.Rotating:
                currentState = EnemyState.Walking;
                break;

            case EnemyState.Walking:
                currentState = EnemyState.Running;
                break;

            case EnemyState.Running:
                currentState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                currentState = EnemyState.Biting;
                break;

            case EnemyState.Biting:
                currentState = EnemyState.Dead;
                break;

            case EnemyState.Dead:
                currentState = EnemyState.Idle;
                break;

            default:
                currentState = EnemyState.Idle;
                break;
        }
    }

    private void ResetTriggers()
    {
        //anim.ResetTrigger("Idle");
        anim.SetTrigger("Idle");
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
                break;

            case EnemyState.Staggering:
                anim.SetTrigger("Reaction Hit");
                break;

            case EnemyState.Rotating:
                anim.SetTrigger("Rotate");
                break;

            case EnemyState.Walking:
                anim.SetTrigger("Walk");
                break;

            case EnemyState.Running:
                anim.SetTrigger("Walk");
                anim.SetTrigger("Run");
                break;

            case EnemyState.Attack:
                anim.SetTrigger("Idle");
                anim.SetTrigger("Attack");
                break;

            case EnemyState.Biting:
                anim.SetTrigger("Idle");
                anim.SetTrigger("Bite");
                break;

            case EnemyState.Dead:
                anim.SetTrigger("Idle");
                anim.SetTrigger("Death");
                break;

            default:
                currentState = EnemyState.Idle;
                stateChangeTime = 0;
                break;

        }
    }
}
