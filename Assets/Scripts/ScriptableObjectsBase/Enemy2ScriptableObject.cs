using UnityEngine;

[CreateAssetMenu(fileName = "Enemy2", menuName = "ScriptableObjects/Enemy2ScriptableObject")]
public class Enemy2ScriptableObject : EnemyBaseScriptableObject
{
    public float biteProbability = 0.25f;
    public float crawlProbability = 0.25f;
}
