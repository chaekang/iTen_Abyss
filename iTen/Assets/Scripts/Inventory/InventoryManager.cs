using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public Animator inventoryAnimator;
    public Slot[] slots;
    public Transform player;
    public float pickupRange = 2f;
    private bool isInventoryOpen = false;
    private int selectedSlotIndex = -1;
    public LayerMask itemLayer = 8;

    private void Start()
    {
        inventoryUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }

        if (isInventoryOpen)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SelectSlot(i);
                    break;
                }
            }

            if (Input.GetMouseButtonDown(0) && selectedSlotIndex >= 0)
            {
                UseItem(selectedSlotIndex);
            }
        }

        // .아이템 줍기
        if (Input.GetMouseButtonDown(1))
        {
            TryPickupItem();
        }
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (isInventoryOpen)
        {
            inventoryUI.SetActive(true);
            inventoryAnimator.SetBool("IsOpen", true);
        }
        else
        {
            inventoryAnimator.SetBool("IsOpen", false);
            StartCoroutine(DisableInventoryAfterDelay(1f));
        }
    }

    private IEnumerator DisableInventoryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        inventoryUI.SetActive(false);
    }

    private void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Highlight(i == index);
        }
    }

    private void UseItem(int index)
    {
        Slot slot = slots[index];
        if (slot.HasItem)
        {
            slot.UseItem();
            if (slot.ItemCount <= 0)
            {
                slot.ClearSlot();
            }
        }
    }

    private void TryPickupItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red, 0.5f);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, itemLayer))
        {
            Item item = hit.collider.GetComponent<Item>();
            if (item != null)
            {
                AddItemToInventory(item);
                Destroy(item.gameObject);
            }
        }
    }

    private void AddItemToInventory(Item newItem)
    {
        foreach (Slot slot in slots)
        {
            if (slot.HasSameItem(newItem))
            {
                slot.IncreaseItemCount(newItem.Amount);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (!slot.HasItem)
            {
                slot.AddItem(newItem);
                return;
            }
        }
    }
}