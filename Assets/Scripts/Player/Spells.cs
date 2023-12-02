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

    private bool hasFreeze = true;
    public GameObject freezeSpellPrefab;
    private float freezeCooldown = 5;
    private int freezeDamage = 2;
    private float lastCastTimeFreeze = -5;

    private AudioSource audioSource;
    public AudioClip[] audioClips;


    private void Start()
    {
        ShopManager.Instance.onItemPickUpEvent += OnItemPickUp;
        audioSource = GetComponent<AudioSource>();
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

        audioSource.clip = audioClips[0];
        audioSource.Play();
    }

    public void CastFreeze()
    {
        GameObject freezeObj = Instantiate(freezeSpellPrefab, transform.position, transform.rotation);
        lastCastTimeFreeze = Time.time;

        audioSource.clip = audioClips[1];
        audioSource.Play();
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
