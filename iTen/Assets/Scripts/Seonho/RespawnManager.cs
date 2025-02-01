using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance { get; private set; }

    public GameObject gameOverUI;
    public float respawnCountdown = 10f;
    public InventoryManager inventoryManager;
    public TextMeshProUGUI countdownText;

    private bool isWaitingForInput = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerDeath()
    {
        gameOverUI.SetActive(true);

        if (countdownText != null)
        {
            countdownText.text = "Press F to respawn";
        }

        isWaitingForInput = true;
    }

    private void Update()
    {
        if (isWaitingForInput && Input.GetKeyDown(KeyCode.F))
        {
            isWaitingForInput = false;

            StartCoroutine(RespawnPlayer());
        }
    }

    private IEnumerator RespawnPlayer()
    {
        if (inventoryManager != null)
        {
            foreach (Slot slot in inventoryManager.slots)
            {
                slot.ClearSlot();
            }
        }

        float countdown = respawnCountdown;
        while (countdown > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(countdown).ToString();
            }
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        if (countdownText != null)
        {
            countdownText.text = "";
        }

        gameOverUI.SetActive(false);
    }
}
