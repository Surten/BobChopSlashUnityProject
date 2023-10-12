using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class ShopManager : MonoBehaviour
{
    public List<Item> avaliableItems = new List<Item>();
    public Dictionary<Item, int> ownedItems = new Dictionary<Item, int>();
    public ShopButtonBuy b1, b2, b3;
    System.Random rnd;
    public TextMeshProUGUI coinText;

    public Transform ownedItemsParent;
    InventorySlot[] inventorySlots;

    private int coins = 0;
    private int shopRefreshCost = 2;

    public event Action<Item> onItemPickUpEvent;

    #region Singleton
    private static ShopManager mInstance;

    private void Awake()
    {
        if (mInstance != null)
            Debug.Log("More than one instance of Inventory");
        mInstance = this;
    }
    public static ShopManager Instance
    {
        get
        {
            return mInstance;
        }
    }
    #endregion


    private void Start()
    {
        rnd = new System.Random();
        LoadNewItemsToShop();

        inventorySlots = ownedItemsParent.GetComponentsInChildren<InventorySlot>();

    }

    public void AddItem(Item item)
    {
        if (!ownedItems.TryAdd(item, 1))
        {
            ownedItems[item] += 1;
        }
        onItemPickUpEvent?.Invoke(item);
        UpdateOwnedItems();
    }

    public void UpdateOwnedItems()
    {
        int i = 0;
        foreach (var (item, count) in ownedItems)
        {
            inventorySlots[i].AddItem(item, count);
            i++;
        }
    }


    public void AddCoins(int addedCoins)
    {
        coins += addedCoins;
        coinText.text = coins.ToString();
    }

    public bool RemoveCoins(int removedCoins)
    {
        if (coins < removedCoins) return false;
        coins -= removedCoins;
        coinText.text = coins.ToString();
        return true;
    }

    public int GetCoins() { return coins; }

    public void RefreshShop()
    {
        if (RemoveCoins(shopRefreshCost))
        {
            LoadNewItemsToShop();
        }
        else
        {
            //let know that he is poor somehow visually
            Debug.Log("You are too poor");
        }
    }

    private void LoadNewItemsToShop()
    {
        b1.LoadNewItem(avaliableItems[rnd.Next(avaliableItems.Count)]);
        b2.LoadNewItem(avaliableItems[rnd.Next(avaliableItems.Count)]);
        b3.LoadNewItem(avaliableItems[rnd.Next(avaliableItems.Count)]);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddCoins(2);
        }
    }
}
