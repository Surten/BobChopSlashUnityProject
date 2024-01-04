using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worlf : MonoBehaviour
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

    public void Bite()
    {
        if (!isAttacking)
        {
            StartCoroutine(Biting());
        }

    }

    public void Chow()
    {
        if (!isAttacking)
        {
            StartCoroutine(Chowing());
        }

    }

    IEnumerator Biting()
    {
        PlayAnimation("Bite");
        Debug.Log("Biting");
        isAttacking = true;
        yield return new WaitForSeconds(1.35f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }

    IEnumerator Chowing()
    {
        PlayAnimation("Chow");
        Debug.Log("Chowing");
        isAttacking = true;
        yield return new WaitForSeconds(1.5f);
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