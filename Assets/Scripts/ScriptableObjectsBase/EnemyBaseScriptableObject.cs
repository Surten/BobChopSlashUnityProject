using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBase", menuName = "ScriptableObjects/EnemyBaseScriptableObject")]
public class EnemyBaseScriptableObject : ScriptableObject
{
    public int maxHealth = 20;
    public int coinsDropOnDeath = 2;

    public float rotateSpeed = 360f;
    public float walkSpeed = 3f;
    public float runSpeed = 9f;
    public float movingMemoryFrame = 1f;
    public float forgottenMemoryFrame = 3f;
    public float awarenessAwareRange = 40f;
    public float awarenessChaseRange = 15f;
    public float awarenessAttackRange = 2f;

    public int attackDamage = 1;
    public float attackRadius = 1f;

    public float chargeProbability = 0.25f;
    public float kamikazeProbability = 0.25f;
    public float staggerProbability = 0.50f;

    public float despawnTime = 1f;
    public float quickDespawnTime = 0.2f;
    public float staggerTime = 3f;

    public float fieldOfViewAngle = 45f;

}
