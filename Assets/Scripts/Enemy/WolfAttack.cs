using System.Collections;
using UnityEngine;

public class WolfAttack: EnemyAttack
{
    /* Attacks */
    public void Bite()
    {
        if (!isAttacking)
        {
            attackRoutine = StartCoroutine(Biting());
        }

    }

    public void Chow()
    {
        if (!isAttacking)
        {
            attackRoutine = StartCoroutine(Chowing());
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
}
