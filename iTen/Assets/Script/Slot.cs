using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Slot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemCountText;
    public Item currentItem;
    private int itemCount;

    public bool HasItem => currentItem != null;
    public int ItemCount => itemCount;

    public void AddItem(Item item)
    {
        currentItem = item;
        itemCount = item.Amount;
        UpdateUI();
    }

    public void IncreaseItemCount(int amount)
    {
        if (HasItem)
        {
            itemCount += amount;
            UpdateUI();
        }
    }

    public void UseItem()
    {
        if (itemCount > 0)
        {
            itemCount--;
            UpdateUI();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;
        UpdateUI();
    }

    public bool HasSameItem(Item item) => currentItem != null && currentItem.itemName == item.itemName;

    public void Highlight(bool isSelected)
    {
        itemImage.color = isSelected ? Color.gray : Color.black;
    }

    private void UpdateUI()
    {
        itemImage.sprite = currentItem != null ? currentItem.icon : null;
        itemImage.enabled = currentItem != null;

        if (HasItem)
        {
            itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
        }
        else
        {
            itemCountText.text = "";
        }

        itemCountText.enabled = itemCount > 1 || currentItem != null;
    }
}