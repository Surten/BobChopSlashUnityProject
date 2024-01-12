using System.Collections;
using UnityEngine;

public class HumanoidAttack : EnemyAttack
{
    /* Attacks */
    public void Punch()
    {
        if (!isAttacking)
        {
            attackRoutine = StartCoroutine(Punching());
        }

    }

    public void Kick() 
    {
        if (!isAttacking) 
        {
            attackRoutine = StartCoroutine(Kicking());
        }
    }

    public void Choke()
    {
        if (!isAttacking)
        {
            StartCoroutine(Choking());
        }

    }

    IEnumerator Punching()
    {
        PlayAnimation("Attack");
        isAttacking = true;
        yield return new WaitForSeconds(2f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }

    IEnumerator Kicking()
    {
        PlayAnimation("Kick");
        isAttacking = true;
        yield return new WaitForSeconds(1.35f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }

    IEnumerator Choking()
    {
        PlayAnimation("Choke");
        isAttacking = true;
        yield return new WaitForSeconds(6.2f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }
}
