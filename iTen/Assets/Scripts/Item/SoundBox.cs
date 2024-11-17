using UnityEngine;

public class SoundBox : ItemObject
{
    public Transform player;

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
        }
    }
}
