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

    public GameObject fadeImage;
    public float fadeDuration = 2f;

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

        if (fadeImage != null)
        {
            var color = fadeImage.GetComponent<SpriteRenderer>().color;
            color.a = 0f;
            fadeImage.GetComponent<SpriteRenderer>().color = color;
        }
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(cutsceneKey) && !isCutsceneActive)
    //     {
    //         StartCoroutine(PlayCutscene());
    //     }
    // }

    public IEnumerator PlayCutscene()
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

        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeOut());

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

    private IEnumerator FadeOut()
    {
        if (fadeImage != null)
        {
            float elapsedTime = 0f;
            var color = fadeImage.GetComponent<SpriteRenderer>().color;
            while (elapsedTime < fadeDuration)
            {
                color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                fadeImage.GetComponent<SpriteRenderer>().color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            color.a = 1f;
            fadeImage.GetComponent<SpriteRenderer>().color = color;
        }
    }
}