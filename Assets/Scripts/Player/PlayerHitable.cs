using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHitable : Hitable
{
    /*    public PlayerManager playerManager;*/
    public PlayerScriptableObject playerScriptableObject;
    public Slider healthBar;


    private void Start()
    {
        healthBar.maxValue = maxHealth = currentHealth = playerScriptableObject.maxHealth;
        healthBar.value = currentHealth;

        ShopManager.Instance.onItemPickUpEvent += OnItemPickUp;
    }

    public override void TakeDamage(int value)
    {
        //todo sound effect

        if (currentHealth <= 0) return;
        base.TakeDamage(value);
        UpdateHealthBar();

        //die lol
        if (currentHealth <= 0) Die();
    }

    public override void Heal(int value)
    {

        //invoke event for other scripts to use
        base.Heal(value);
        UpdateHealthBar();
    }

    protected override void Die()
    {
        base.Die();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().mass = 0.1f;
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(2);
        }
    }

    private void OnItemPickUp(Item item)
    {
        IncreaseMaxHealth(item.bonusMaxHealth);
        IncreaseArmor(item.bonusArmor);
    }

    private void UpdateHealthBar()
    {
        healthBar.value = currentHealth;
    }
    private void IncreaseMaxHealth(int increaseValue)
    {
        healthBar.maxValue = maxHealth += increaseValue;
        healthBar.value = currentHealth += increaseValue;
    }

    private void IncreaseArmor(int increaseValue)
    {
        armor += increaseValue;
    }

}
