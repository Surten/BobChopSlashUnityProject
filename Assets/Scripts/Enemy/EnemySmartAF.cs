using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySmartAF : MonoBehaviour
{
    private float rotateSpeed;
    private float walkSpeed;
    private float awarenessAwareRange;
    private float awarenessChaseRange;
    private float awarenessAttackRange;

    private float attackRadius;
    private int attackDamage;

    private EnemyState currentState = EnemyState.Idle;

    public Enemy1ScriptableObject enemyScriptableObject;
    public Transform target;
    public AttackMelee attackMelee;


/*    public Transform groundCheck;

    private bool grounded = false;

    public LayerMask groundLayerMask;*/



    public enum EnemyState{ Idle, Rotating, Walking, Attack, Dead }
    private void Start()
    {
        rotateSpeed = enemyScriptableObject.rotateSpeed;
        walkSpeed = enemyScriptableObject.walkSpeed;
        awarenessAwareRange = enemyScriptableObject.awarenessAwareRange;
        awarenessChaseRange = enemyScriptableObject.awarenessChaseRange;
        awarenessAttackRange = enemyScriptableObject.awarenessAttackRange;

        attackMelee.attackRadius = attackRadius = enemyScriptableObject.attackRadius;
        attackMelee.attackDamage = attackDamage = enemyScriptableObject.attackDamage;
    }



/*    void Jump()
    {
        grounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayerMask);
        if (grounded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 4f, ForceMode.Impulse);
        }
    }*/

    public void SetNewTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetEnemyState(EnemyState state)
    {
        currentState = state;
    }
    public EnemyState GetEnemyState()
    {
        return currentState;
    }

    private void Update()
    {

        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Rotating:
                RotateToTarget();
                break;

            case EnemyState.Walking:
                RotateToTarget();
                WalkingToPosition();
                break;

            case EnemyState.Attack:
                RotateToTarget();
                attackMelee.SwingSword();
                break;

        }

    }


    private void WalkingToPosition()
    {
        transform.position += transform.forward * Time.deltaTime * walkSpeed;
/*        if (transform.position.y < target.position.y - 0.5f)
        {
            Jump();
        }*/
    }

    private void RotateToTarget()
    {
        Vector3 v = target.position - transform.position;
        v.y = 0;
        var q = Quaternion.LookRotation(v);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);
    }
}
