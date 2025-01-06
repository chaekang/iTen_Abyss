using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using System.Net.NetworkInformation;
using System.Collections;

public class SoundMonsterJumpScare : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera jumpScareCam;
    [SerializeField] private Animator cameraAni;
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

        // 카메라 애니메이션 실행
        if (cameraAni != null)
        {
            cameraAni.SetTrigger("PlayJumpScare");
        }

        // 실행 동안 기다림
        yield return new WaitForSeconds(scareDuration);

        // 일반 카메라로 전환
        jumpScareCam.Priority = 9;

        // 종료
        isScaring = false;
    }
}
