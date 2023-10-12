using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerScriptableObject", order = 1)]
public class PlayerScriptableObject : ScriptableObject
{
    public int maxHealth = 50;
    public float moveSpeed = 5f;
    public int attackDamage = 4;
}
