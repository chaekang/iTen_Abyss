using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class SoundMonsterJumpScare : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera jumpScareCam;
    [SerializeField] private PostProcessVolume postProcessing;
    [SerializeField] private GameObject bloodImage;
    [SerializeField] private float scareDuration = 3f;

    [SerializeField] private bool isScaring = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && !isScaring)
        {
            Debug.Log("Start JumpScare");
            StartCoroutine(TriggerJumpScare());
        }
    }

    private IEnumerator TriggerJumpScare()
    {
        isScaring = true;

        // 점프스케어 카메라 활성화
        jumpScareCam.Priority = 11;

        // 실행 동안 기다림
        yield return new WaitForSeconds(scareDuration - 0.75f);

        // 피 튀기는 효과 실행
        bloodImage.SetActive(true);
        yield return new WaitForSeconds(0.75f);

        // 일반 카메라로 전환
        jumpScareCam.Priority = 9;

        // 종료
        isScaring = false;
        yield return new WaitForSeconds(3f);
        bloodImage.SetActive(false);
    }

    private void Update()
    {
        if (jumpScareCam.Priority >= 11)
        {
            postProcessing.enabled = true;
        }
        else
        {
            postProcessing.enabled = false;
        }
    }
}
