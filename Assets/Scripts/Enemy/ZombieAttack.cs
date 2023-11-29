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

    public bool attackMelee = false;


    public void SwingArm()
    {
        if (!attackMelee)
        {
            StartCoroutine(Attacking());
        }

    }

    public void Bite()
    {
        if (!attackMelee)
        {
            StartCoroutine(Biting());
        }

    }

    IEnumerator Attacking()
    {
        anim.SetTrigger("Transition I");
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Attack");
        attackMelee = true;
        yield return new WaitForSeconds(0.7f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        attackMelee = false;
        anim.SetTrigger("Transition II");
        yield return new WaitForSeconds(0.5f);

    }

    IEnumerator Biting()
    {
        anim.SetTrigger("Transition I");
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Bite");
        attackMelee = true;
        yield return new WaitForSeconds(0.7f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        attackMelee = false;
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
