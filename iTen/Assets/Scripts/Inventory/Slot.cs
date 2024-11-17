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

    private void Start()
    {
        UpdateUI();
    }

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
        if (currentItem != null)
        {
            itemImage.sprite = currentItem.icon;
            itemImage.enabled = true;
            itemImage.color = Color.white;

            itemCountText.text = itemCount > 1 ? itemCount.ToString() : "";
            itemCountText.enabled = true;

        }
        else
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
            itemCountText.text = "";
            itemCountText.enabled = false;
        }
    }
}