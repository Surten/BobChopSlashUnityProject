using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image icon;
    private TextMeshProUGUI numOfItemsText;
    private Item item;

    private int itemCount;

    private void Start()
    {
        icon = GetComponent<Image>();
        numOfItemsText = GetComponentInChildren<TextMeshProUGUI>();
        numOfItemsText.text = "";
    }

    public void AddItem(Item i, int count)
    {
        item = i;
        icon.sprite = i.icon;
        numOfItemsText.text = count.ToString();
    }


    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
    }
}
