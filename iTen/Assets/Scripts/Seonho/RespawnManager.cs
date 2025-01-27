using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class RespawnManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public Transform respawnPoint;
    public float respawnCountdown = 10f;
    public FirstPersonController playerController; 
    public InventoryManager inventoryManager;
    public TextMeshProUGUI countdownText;
    private Button respawnButton;

    private void Start()
    {
        gameOverUI.SetActive(false);
        if (gameOverUI != null)
        {
            respawnButton = gameOverUI.GetComponentInChildren<Button>();
        }
        if (countdownText != null)
        {
            countdownText.text = "";
        }
    }

    public void OnPlayerDeath()
    {
        gameOverUI.SetActive(true);

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (respawnButton != null)
        {
            respawnButton.gameObject.SetActive(true);
            respawnButton.interactable = true;
            respawnButton.onClick.RemoveAllListeners();
            respawnButton.onClick.AddListener(() => StartCoroutine(RespawnPlayer()));
        }

        if (countdownText != null)
        {
            countdownText.text = "";
        }
    }

    private IEnumerator RespawnPlayer()
    {
        if (respawnButton != null)
        {
            respawnButton.interactable = false;
            respawnButton.gameObject.SetActive(false);
        }

        if (inventoryManager != null)
        {
            foreach (Slot slot in inventoryManager.slots)
            {
                slot.ClearSlot();
            }
        }

        if (playerController != null && respawnPoint != null)
        {
            playerController.transform.position = respawnPoint.position;
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

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}
