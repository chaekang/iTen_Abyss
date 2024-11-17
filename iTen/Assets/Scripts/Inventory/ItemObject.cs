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
}
