using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCutscene : MonoBehaviour
{
    public Camera playerCamera;
    public Camera cutsceneCamera;
    public Animator doorAnimator;
    public GameObject player;
    public KeyCode cutsceneKey = KeyCode.Tab;

    public AudioClip cutsceneSound;
    private AudioSource audioSource;

    private FirstPersonController playerController;
    private bool isCutsceneActive = false;

    private void Start()
    {
        if (player != null)
        {
            playerController = player.GetComponent<FirstPersonController>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(cutsceneKey) && !isCutsceneActive)
        {
            StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene()
    {
        isCutsceneActive = true;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        playerCamera.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }

        PlayCutsceneSound();

        yield return new WaitForSeconds(3f);

        cutsceneCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isCutsceneActive = false;
    }

    private void PlayCutsceneSound()
    {
        if (cutsceneSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(cutsceneSound);
        }
    }
}