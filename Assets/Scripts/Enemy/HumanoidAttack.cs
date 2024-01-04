using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAttack : MonoBehaviour
{

    public Animator anim;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public int attackDamage = 5;

    public LayerMask layerMask;

    public bool isAttacking = false;


    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void SwingArm()
    {
        if (!isAttacking)
        {
            StartCoroutine(Attacking());
        }

    }

    public void Kick() 
    {
        if (!isAttacking) 
        {
            StartCoroutine(Kicking());
        }
    }

    public void Choke()
    {
        if (!isAttacking)
        {
            StartCoroutine(Choking());
        }

    }

    IEnumerator Attacking()
    {
        PlayAnimation("Attack");
        Debug.Log("Attacking");
        isAttacking = true;
        yield return new WaitForSeconds(2f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }

    IEnumerator Kicking()
    {
        PlayAnimation("Kick");
        Debug.Log("Kick");
        isAttacking = true;
        yield return new WaitForSeconds(1.35f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }

    IEnumerator Choking()
    {
        PlayAnimation("Choke");
        Debug.Log("Kick");
        isAttacking = true;
        yield return new WaitForSeconds(6.2f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }

    void PlayAnimation(string stateName)
    {
        // Play the specified animation
        anim.Play(stateName, 0, 0f);
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
