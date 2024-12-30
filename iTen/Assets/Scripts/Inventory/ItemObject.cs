using UnityEditor.Rendering;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;
    private void Start()
    {
        if (itemData != null)
        {
            amount = itemData.defaultAmount;
        }
    }

    public virtual void OnDrop()
    {

    }

    public static void CheckAndInteract()
    {
        float pickupRange = 5f;
        LayerMask itemLayer = 1 << 8;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Debug.DrawRay(ray.origin, ray.direction * pickupRange, Color.blue, 0.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, itemLayer))
        {
            ItemObject item = hit.collider.GetComponent<ItemObject>();
            if (item != null)
            {
                item.OnInteract();
            }
        }
    }

    public virtual void OnInteract()
    {

    }
}
