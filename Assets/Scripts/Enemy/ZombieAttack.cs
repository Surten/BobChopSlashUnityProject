using System.Collections;
using UnityEngine;

public class ZombieAttack : EnemyAttack
{
    /* Attacks */
    public void SwingArm()
    {
        if (!isAttacking)
        {
            attackRoutine = StartCoroutine(ArmSwinging());
        }

    }

    public void Bite()
    {
        if (!isAttacking)
        {
            attackRoutine = StartCoroutine(Biting());
        }

    }

    IEnumerator ArmSwinging()
    {
        PlayAnimation("Attack");
        isAttacking = true;
        yield return new WaitForSeconds(4f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }

    IEnumerator Biting()
    {
        PlayAnimation("Bite");
        isAttacking = true;
        yield return new WaitForSeconds(4f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;
    }
}
