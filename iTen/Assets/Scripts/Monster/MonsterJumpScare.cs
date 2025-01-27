using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class MonsterJumpScare : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera jumpScareCam;
    [SerializeField] private PostProcessVolume postProcessing;

    [SerializeField] private Transform[] targetPos;
    [SerializeField] private CinemachineSmoothPath[] dollyTracks;
    [SerializeField] private CinemachineDollyCart dollyCart;

    [SerializeField] private GameObject bloodImage;
    [SerializeField] private float scareDuration = 3f;

    [SerializeField] private bool isScaring = false;

    private int curTargetIndex = -1;

    private void Start()
    {
        Transform jumpScareRoot = GameObject.Find("JumpScareRoot")?.transform;
        Transform jumpScareTarget = GameObject.Find("JumpScareTarget")?.transform;

        targetPos = new Transform[] { jumpScareRoot, jumpScareTarget };

        if (targetPos[0] == null || targetPos[1] == null)
        {
            Debug.Log("JumpScareRoot 또는 JumpScareTarget을 찾지 못했습니다.");
        }
    }

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

        // 가까운 타겟으로 설정
        int closestTargetIndex = GetClosetTargetIndex();
        if (curTargetIndex != closestTargetIndex)
        {
            SwitchToTarget(closestTargetIndex);
        }

        // 점프스케어 카메라 활성화
        jumpScareCam.Priority = 11;

        // 카메라 이동
        MoveDollyCart();

        // 사운드 실행
        string monsterIndex = "JumpScare" + GetClosetTargetIndex();
        SoundManager.Instance.PlayJumpScareSound(monsterIndex);

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

        //리스폰
        if (RespawnManager.Instance != null)
        {
            RespawnManager.Instance.OnPlayerDeath();
        }
        else
        {
            Debug.LogError("RespawnManager Instance not found in the scene.");
        }
    }

    private int GetClosetTargetIndex()
    {
        float disToTarget0 = Vector3.Distance(transform.position, targetPos[0].position);
        float disToTarget1 = Vector3.Distance(transform.position, targetPos[1].position);

        return disToTarget0 < disToTarget1 ? 0 : 1;

    }

    private void SwitchToTarget(int newTargetIndex)
    {
        curTargetIndex = newTargetIndex;

        jumpScareCam.LookAt = targetPos[curTargetIndex];

        dollyCart.m_Path = dollyTracks[curTargetIndex];
        dollyCart.m_Position = 0;
    }

    private void MoveDollyCart()
    {
        if (dollyCart.m_Path != null)
        {
            dollyCart.m_Position += Time.deltaTime;
        }
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
