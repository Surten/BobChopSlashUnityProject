using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/ItemScriptableObject")]

public class Item : ScriptableObject
{
    public string itemName = "newItem";
    public int id = 0;
    public Sprite icon;
    
    [TextArea]
    public string description = "newDescription";


    public float bonusMovementSpeed = 0;
    public int bonusMaxHealth = 0;
    public int bonusArmor = 0;
    public int bonusAttackDamage = 0;

    public int addFireBallDamage = 0;
    public float reduceFireBallCooldownbyPercent = 0;

    public bool freezeSpell = false;



    public int price;

}
