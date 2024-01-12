using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleAttack : EnemyAttack
{
    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    /* Attacks */
    public void Attack()
    {
        if (!isAttacking)
        {
            attackRoutine = StartCoroutine(Attacking());
        }

    }

    IEnumerator Attacking()
    {
        isAttacking = true;
        yield return new WaitForSeconds(2f);
        ResolveHit();
        yield return new WaitForSeconds(0.3f);
        isAttacking = false;

    }
}
