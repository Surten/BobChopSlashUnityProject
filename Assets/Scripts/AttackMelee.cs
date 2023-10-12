using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMelee : MonoBehaviour
{

    public Animator anim;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public int attackDamage = 5;

    public LayerMask layerMask;

    public bool swingingSwordCR = false;


    public void SwingSword()
    {
        if (!swingingSwordCR)
        {
            StartCoroutine(Attacking());
        }


    }

    IEnumerator Attacking()
    {
        anim.SetTrigger("swordSlash");
        swingingSwordCR = true;
        yield return new WaitForSeconds(0.7f);
        ResolveSwordHit();
        yield return new WaitForSeconds(0.3f);
        swingingSwordCR = false;
    }

    void ResolveSwordHit()
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
