using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spells : MonoBehaviour
{
    private bool hasFireball = false;
    public GameObject fireballPrefab;
    private float fireballCooldown = 3;
    private int fireballDamage = 0;
    private float lastCastTimeFireball = -5;

    private bool hasFreeze = false;
    public GameObject freezeSpellPrefab;
    private float freezeCooldown = 5;
    private int freezeDamage = 2;
    private float lastCastTimeFreeze = -5;


    private void Start()
    {
        ShopManager.Instance.onItemPickUpEvent += OnItemPickUp;
    }

    private void OnItemPickUp(Item item)
    {
        fireballDamage += item.addFireBallDamage;
        hasFireball = (fireballDamage > 0);
        fireballCooldown -= fireballCooldown * (item.reduceFireBallCooldownbyPercent / 100);

        if (!hasFreeze) hasFreeze = item.freezeSpell;
        if (item.freezeSpell) ShopManager.Instance.RemoveItemFromSoldItems(item);
    }
    public void CastFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, transform.position + transform.forward, Quaternion.identity);
        fireball.GetComponent<FireballSpell>().damage = fireballDamage;

        fireball.GetComponent<Rigidbody>().AddForce(transform.forward * 100);

        lastCastTimeFireball = Time.time;

    }

    public void CastFreeze()
    {
        GameObject freezeObj = Instantiate(freezeSpellPrefab, transform.position, transform.rotation);
        lastCastTimeFreeze = Time.time;

    }

    private void Update()
    {

        if (hasFireball && Input.GetKeyDown(KeyCode.F) && lastCastTimeFireball + fireballCooldown < Time.time)
        {
            CastFireball();
        }

        if (hasFreeze && Input.GetKeyDown(KeyCode.G) && lastCastTimeFreeze + freezeCooldown < Time.time)
        {
            CastFreeze();
        }
    }
}
