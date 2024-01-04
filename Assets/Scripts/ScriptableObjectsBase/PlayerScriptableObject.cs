using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerScriptableObject", order = 1)]
public class PlayerScriptableObject : ScriptableObject
{
    public int maxHealth = 50;
    public float moveSpeed = 100f;
    public int attackDamage = 4;
    public int bonusHpOnLevelUp = 5;
}
