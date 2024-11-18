using UnityEngine;

public class SoundBox : ItemObject
{
    private Transform player;
    public AudioClip clip;
    private AudioSource audioSource;
    private bool isDropped = false;
    public bool IsDropped => isDropped;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        audioSource=gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
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
        if (clip != null)
        {
            audioSource.Play();
            Invoke(nameof(StopAudioAndDestroy), 10f);
        }
        else
        {
            Debug.Log("Clip is null");
        }
    }

    private void StopAudioAndDestroy()
    {
        audioSource.Stop();
        Destroy(gameObject);
    }
}
