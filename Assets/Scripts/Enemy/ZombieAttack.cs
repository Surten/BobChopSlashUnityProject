using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{

    public Animator anim;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public int attackDamage = 5;

    public LayerMask layerMask;

    public bool isAttacking = false;


    public void SwingArm()
    {
        if (!isAttacking)
        {
            StartCoroutine(Attacking());
        }

    }

    public void Bite()
    {
        if (!isAttacking)
        {
            StartCoroutine(Biting());
        }

    }

    IEnumerator Attacking()
    {
        anim.SetTrigger("Transition I");
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Attack");
        isAttacking = true;
        yield return new WaitForSeconds(0.7f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
        anim.SetTrigger("Transition II");
        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator Biting()
    {
        anim.SetTrigger("Transition I");
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Bite");
        isAttacking = true;
        yield return new WaitForSeconds(0.7f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
        anim.SetTrigger("Transition II");
        yield return new WaitForSeconds(0.5f);

    }

    void ResolveHit()
    {
        Collider[] enemiesHit = Physics.OverlapSphere(attackPoint.position, attackRadius, layerMask);
        foreach (Collider enemy in enemiesHit)
        {
            enemy.GetComponent<Hitable>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
