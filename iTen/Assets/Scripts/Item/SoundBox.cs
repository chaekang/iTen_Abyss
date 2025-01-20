using UnityEngine;

public class SoundBox : ItemObject
{
    private Transform player;
    private bool isDropped = false;
    public bool IsDropped => isDropped;
    private float soundRange = 100f;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    public override void OnDrop()
    {
        if (itemData.prefab == null)
        {
            Debug.LogError("ItemData prefab is not assigned!");
            return;
        }

        Vector3 dropPosition = player.position + player.forward * 3f;

        GameObject droppedItem = Instantiate(itemData.prefab, dropPosition, Quaternion.identity);
        ItemObject droppedItemObject = droppedItem.GetComponent<ItemObject>();
        if (droppedItemObject != null)
        {
            droppedItemObject.amount = amount;
            if (droppedItemObject is SoundBox soundbox)
            {
                soundbox.isDropped = true;
            }
        }
    }

    public override void OnInteract()
    {
        
        Debug.Log("music box interact");
        if (SoundManager.Instance != null)
        {
            itemData.isUsed = true;
            SoundManager.Instance.PlaySound(transform, soundRange, "SoundBox");
        }
    }
}
