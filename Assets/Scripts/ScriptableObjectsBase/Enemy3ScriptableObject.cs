using UnityEngine;

[CreateAssetMenu(fileName = "Enemy3", menuName = "ScriptableObjects/Enemy3ScriptableObject")]
public class Enemy3ScriptableObject : EnemyBaseScriptableObject
{
    public float kickProbability = 0.25f;
    public float chokeProbability = 0.25f;
}
