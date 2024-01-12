using UnityEngine;

public class EnemyAttack : MonoBehaviour 
{ 

    public Animator anim;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public int attackDamage = 5;

    public LayerMask layerMask;

    public bool isAttacking = false;
    protected Coroutine attackRoutine;

    /* Start Functions*/
    protected void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    /* Animation Functions*/
    protected void PlayAnimation(string stateName)
    {
        // Play the specified animation
        anim.Play(stateName, 0, 0f);
    }

    /* Damage */
    protected void ResolveHit()
    {
        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRadius, layerMask);
        foreach (Collider enemy in enemiesHit)
        {
            enemy.GetComponent<Hitable>().TakeDamage(attackDamage);
        }
    }
    public void StopAttack()
    {
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }


}

