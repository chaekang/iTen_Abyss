using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUI;
    public Animator inventoryAnimator;
    private bool isInventoryOpen = false;

    public Slot[] slots;
    private int selectedSlotIndex = -1;

    public Transform player;
    public float pickupRange = 2f;
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

            if (Input.GetMouseButtonDown(1) && selectedSlotIndex >= 0)
            {
                Debug.Log("아이템 사용 시도");
                UseItem();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryPickupItem();
        }
        if (Input.GetMouseButtonDown(0))
        {
            ItemObject.CheckAndInteract();
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

    private void UseItem()
    {
        // selectedSlotIndex가 유효한 값인지 확인
        if (selectedSlotIndex >= 0 && selectedSlotIndex < slots.Length)
        {
            Slot selectedSlot = slots[selectedSlotIndex];

            // 슬롯이 null인지 확인
            if (selectedSlot == null)
            {
                Debug.LogWarning("선택된 슬롯이 null입니다.");
                return;
            }

            // 슬롯의 itemData가 null인지 확인
            if (selectedSlot.itemData == null)
            {
                Debug.LogWarning("선택된 슬롯에 아이템이 없습니다.");
                return;
            }

            // 아이템을 사용하는 로그
            Debug.Log($"아이템 사용 시도: {selectedSlot.itemData.itemName}");

            // 아이템을 사용
            GameObject user = gameObject;
            selectedSlot.UseItem(user);

            // 사용 후 디버그 로그
            Debug.Log($"{selectedSlot.itemData.itemName} 아이템이 사용되었습니다.");
        }
        else
        {
            Debug.LogWarning("잘못된 슬롯 인덱스입니다.");
        }
    }

    private void TryPickupItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.red, 0.5f);
        
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, itemLayer))
        {
            ItemObject item = hit.collider.GetComponent<ItemObject>();
            if (item != null)
            {
                if (item is SoundBox soundBox && soundBox.IsDropped)
                {
                    Debug.Log("Cannot pick up a dropped SoundBox.");
                    return;
                }

                Debug.Log($"Picking up item: {item.name}");
                bool isAdded = AddItemToInventory(item.itemData, item.amount);
                if (isAdded)
                {
                    Destroy(item.gameObject);
                    Debug.Log($"{item.name} has been destroyed.");
                }
                else
                {
                    Debug.Log("Failed to add item to inventory");
                }
            }
        }
    }

    private bool AddItemToInventory(ItemData itemData, int amount)
    {
        if (itemData == null)
        {
            Debug.Log("itemdata prefab is null");
            return false;
        }

        foreach (Slot slot in slots)
        {
            if (slot.HasSameItem(itemData))
            {
                slot.IncreaseItemCount(amount);
                return true;
            }
        }

        foreach (Slot slot in slots)
        {
            if (!slot.HasItem)
            {
                slot.AddItem(itemData, amount);
                return true;
            }
        }
        return false;
    }
}