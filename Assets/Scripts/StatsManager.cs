using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour
{


    private int maxHP;
    private float attackDamage;
    private float attackSpeed;
    private float attackRadius;
    private float movementSpeed;
    private int armor;
    private int fireballDamage;
    private int freezeSpell;

    public PlayerScriptableObject playerInfo;

    public TextMeshProUGUI textMaxHP; 
    public TextMeshProUGUI textAttackDamage; 
    public TextMeshProUGUI textAttackSpeed; 
    public TextMeshProUGUI textAttackRadius; 
    public TextMeshProUGUI textMovementSpeed; 
    public TextMeshProUGUI textArmor; 
    public TextMeshProUGUI textFireballDamage; 
    public TextMeshProUGUI textFreezeSpell;

    #region Singleton
    private static StatsManager mInstance;

    private void Awake()
    {
        if (mInstance != null)
            Debug.Log("More than one instance of Inventory");
        mInstance = this;
    }
    public static StatsManager Instance
    {
        get
        {
            return mInstance;
        }
    }
    #endregion
    private void Start()
    {
        ShopManager.Instance.onItemPickUpEvent += AddItemToStats;
        PlayerLeveler.Instance.onLevelUpEvent += OnLevelUp;

        maxHP = playerInfo.maxHealth;
        attackDamage = 100;
        attackSpeed = 100;
        attackRadius = 100;
        movementSpeed = 100;
        armor = 0;
        fireballDamage = 0;
        freezeSpell = 0;

        UpdateStatTexts();
}

    public void UpdateStatTexts()
    {
        textMaxHP.text = maxHP.ToString();
        textAttackDamage.text = attackDamage.ToString();
        textAttackSpeed.text = attackSpeed.ToString();
        textAttackRadius.text = attackRadius.ToString();
        textMovementSpeed.text = movementSpeed.ToString();
        textArmor.text = armor.ToString();
        textFireballDamage.text = fireballDamage.ToString();
        textFreezeSpell.text = freezeSpell.ToString();
    }

    private void AddItemToStats(Item item)
    {
        maxHP += item.bonusMaxHealth;
        attackDamage += item.bonusAttackDamage;
        attackSpeed += item.bonusAttackSpeedPercentage;
        attackRadius += item.bonusAttackRadiusPercentage;
        movementSpeed += item.bonusMovementSpeed;
        armor += item.bonusArmor;
        fireballDamage += item.addFireBallDamage;
        freezeSpell = (item.freezeSpell || freezeSpell == 1) ? 1 : 0;

        UpdateStatTexts();
}

    private void OnLevelUp()
    {
        maxHP += playerInfo.bonusHpOnLevelUp;
        UpdateStatTexts();
    }


}
