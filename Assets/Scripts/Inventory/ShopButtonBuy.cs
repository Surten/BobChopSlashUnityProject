using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class ShopButtonBuy : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI description;
    public Image spriteImage;

    private Item currentItem;
    private ShopManager shopManager;

    private void Start()
    {
        shopManager = ShopManager.Instance;
    }

    public void LoadNewItem(Item item)
    {
        currentItem = item;
        itemName.text = item.itemName;
        description.text = item.description;
        spriteImage.enabled = true;
        spriteImage.sprite = item.icon;
    }

    public void BuyCurrentItem()
    {
        if (currentItem == null)
            return;
        if (!shopManager.RemoveCoins(currentItem.price))
            return;
        shopManager.AddItem(currentItem);

        currentItem = null;

        itemName.text = "SOLD";
        description.text = "";
        spriteImage.enabled = false;
    }
}
