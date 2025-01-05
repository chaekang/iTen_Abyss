using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Slot : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemCountText;
    public ItemData itemData;
    private int itemCount;
    public bool HasItem => itemData != null;
    public int ItemCount => itemCount;

    private void Start()
    {
        UpdateUI();
    }

    public void AddItem(ItemData newItemData, int amount)
    {
        itemData = newItemData;
        itemCount = amount;
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

    public void UseItem(GameObject user)
    {
        if (itemData != null)
        { 
            itemData.action.Use(user);

            Debug.Log($"{itemData.itemName} 아이템이 사용되었습니다.");

            itemCount--;

            if (itemCount <= 0)
            {
                ClearSlot();
            }
        }

        else
        {
            Debug.Log("아이템이 없거나 사용할 수 없습니다.");
        }
    }

    public void ClearSlot()
    {
        itemData = null;
        itemCount = 0;
        UpdateUI();
    }

    public bool HasSameItem(ItemData itemData) => itemData != null && itemData.itemName == itemData.itemName;

    public void Highlight(bool isSelected)
    {
        itemImage.color = isSelected ? Color.gray : Color.black;
    }

    private void UpdateUI()
    {
        if (itemData != null)
        {
            itemImage.sprite = itemData.icon;
            itemImage.enabled = true;
            itemImage.color = Color.white;

            itemCountText.text = itemCount > 1 ? itemCount.ToString() : "1";
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