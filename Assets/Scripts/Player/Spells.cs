using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spells : MonoBehaviour
{
    public bool hasFireball = false;
    public GameObject fireballPrefab;
    public float fireballCooldown = 1;
    private int fireballDamage = 2;
    private float lastCastTime = 0;

    private void Start()
    {
        ShopManager.Instance.onItemPickUpEvent += OnItemPickUp;
    }

    private void OnItemPickUp(Item item)
    {
        fireballDamage += item.addFireBallDamage;
        hasFireball = (fireballDamage > 0);
        fireballCooldown -= fireballCooldown * (item.reduceFireBallCooldownbyPercent / 100);
    }
    public void CastFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, transform.position + transform.forward, Quaternion.identity);
        fireball.GetComponent<FireballSpell>().damage = fireballDamage;
        Vector3 a = transform.forward;
        //a.y = 0.2f;
        fireball.GetComponent<Rigidbody>().AddForce(a * 100);

        lastCastTime = Time.time;

    }

    private void Update()
    {

        if (hasFireball && Input.GetKeyDown(KeyCode.F) && lastCastTime + fireballCooldown < Time.time)
        {
            CastFireball();
        }
    }
}
