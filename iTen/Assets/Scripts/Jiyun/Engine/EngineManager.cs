// using UnityEngine;
// using Photon.Pun;
// using TMPro;
// using System.Collections;
// using StarterAssets;
// using Photon.Realtime;

// public class EngineManager : MonoBehaviourPun
// {
//     public bool engine_1;
//     public bool engine_2;
//     public bool engine_3;
//     public TextMeshProUGUI allClearText; // "All Clear" 텍스트를 표시할 TextMeshProUGUI 컴포넌트

//     public Camera playerCamera;
//     public Camera cutsceneCamera;
//     public Animator doorAnimator;
//     private GameObject player;
//     public KeyCode cutsceneKey = KeyCode.Tab;

//     public AudioClip cutsceneSound;
//     private AudioSource audioSource;

//     private FirstPersonController playerController;
//     private bool isCutsceneActive = false;

//     public void CheckAllEngines()
//     {
//         // 모든 엔진이 true인가
//         if (engine_1 && engine_2 && engine_3)
//         {
            
//         }
//     }

//     public IEnumerator PlayCutscene()
//     {
//         isCutsceneActive = true;

//         if (playerController != null)
//         {
//             playerController.enabled = false;
//         }

//         playerCamera.gameObject.SetActive(false);
//         cutsceneCamera.gameObject.SetActive(true);

//         if (doorAnimator != null)
//         {
//             doorAnimator.SetTrigger("Open");
//         }

//         PlayCutsceneSound();

//         yield return new WaitForSeconds(3f);

//         cutsceneCamera.gameObject.SetActive(false);
//         playerCamera.gameObject.SetActive(true);

//         if (playerController != null)
//         {
//             playerController.enabled = true;
//         }

//         isCutsceneActive = false;
//     }

//     private void PlayCutsceneSound()
//     {
//         if (cutsceneSound != null && audioSource != null)
//         {
//             audioSource.PlayOneShot(cutsceneSound);
//         }
//     }
// }

using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections;
using StarterAssets;
using static Photon.Voice.OpusCodec;

public class EngineManager : MonoBehaviourPun
{
    public bool engine_1;
    public bool engine_2;
    public bool engine_3;
    public TextMeshProUGUI allClearText;

    public Camera cutsceneCamera;
    public Animator doorAnimator;

    public AudioClip cutsceneSound;
    private AudioSource audioSource;

    private bool isCutsceneActive = false;

    public GameObject fadeImage;
    public float fadeDuration = 2f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void CheckAllEngines()
    {
        if (engine_1 && engine_2 && engine_3)
        {
            photonView.RPC("TriggerCutsceneForAllPlayers", RpcTarget.All);
        }
    }

    [PunRPC]
    public void TriggerCutsceneForAllPlayers()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        isCutsceneActive = true;

        // 현재 플레이어의 카메라 및 컨트롤러 비활성화
        var localPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        if (localPlayer != null)
        {
            var playerCamera = localPlayer.GetComponentInChildren<Camera>();
            var playerController = localPlayer.GetComponent<FirstPersonController>();

            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
            }

            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        // 컷씬 카메라 활성화
        cutsceneCamera.gameObject.SetActive(true);

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }

        PlayCutsceneSound();

        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeOut());

        // 컷씬 종료 후 원래 상태로 복원
        cutsceneCamera.gameObject.SetActive(false);

        if (localPlayer != null)
        {
            var playerCamera = localPlayer.GetComponentInChildren<Camera>();
            var playerController = localPlayer.GetComponent<FirstPersonController>();

            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(true);
            }

            if (playerController != null)
            {
                playerController.enabled = true;
            }
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
