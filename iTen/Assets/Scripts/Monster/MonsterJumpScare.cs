using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;

public class MonsterJumpScare : MonoBehaviourPun
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
            Debug.Log("JumpScareRoot  Ǵ  JumpScareTarget   ã      ߽  ϴ .");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && !isScaring)
        {
            // 자신의 PhotonView를 사용하여 RPC 호출
            photonView.RPC("TriggerJumpScareRPC", RpcTarget.All); 
        }
    
    }

[PunRPC]
public void TriggerJumpScareRPC()
{
    StartCoroutine(TriggerJumpScare());
}

    private IEnumerator TriggerJumpScare()
    {
        if (photonView.IsMine && !isScaring)
        {
            isScaring = true;

            //       Ÿ           
            int closestTargetIndex = GetClosetTargetIndex();
            if (curTargetIndex != closestTargetIndex)
            {
                SwitchToTarget(closestTargetIndex);
            }

            //        ɾ  ī ޶  Ȱ  ȭ
            jumpScareCam.Priority = 11;

            // ī ޶   ̵ 
            MoveDollyCart();

            //          
            string monsterIndex = "JumpScare" + GetClosetTargetIndex();
            SoundManager.Instance.PlayJumpScareSound(monsterIndex);

            //             ٸ 
            yield return new WaitForSeconds(scareDuration - 0.75f);

            //    Ƣ    ȿ       
            bloodImage.SetActive(true);
            yield return new WaitForSeconds(0.75f);

            //  Ϲ  ī ޶     ȯ
            jumpScareCam.Priority = 9;

            //     
            isScaring = false;
            yield return new WaitForSeconds(3f);
            bloodImage.SetActive(false);

            //      
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.OnPlayerDeath();
            }
            else
            {
                Debug.LogError("RespawnManager Instance not found in the scene.");
            }
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
