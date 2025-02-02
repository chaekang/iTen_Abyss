using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;

public class Engineone : MonoBehaviourPun
{
    private List<Photon.Realtime.Player> pressingPlayers = new List<Photon.Realtime.Player>();

    public AudioClip engineSound;
    private AudioSource audioSource;

    public void OnMouseDown()
    {
        Debug.Log("Pressing!");
        // 오브젝트 클릭 시작 시 호출
        photonView.RPC("ObjectClicked", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void ObjectClicked(Photon.Realtime.Player player)
    {
        // 클릭한 플레이어를 리스트에 추가
        if (!pressingPlayers.Contains(player))
        {
            pressingPlayers.Add(player);
        }

        // 두 명의 플레이어가 클릭했는가? -> 나중에 세 명으로 바꿀거임
        if (pressingPlayers.Count == 2)
        {
            GameObject engineManagerObject = GameObject.Find("EngineManager");
            EngineManager engineManager1 = engineManagerObject.GetComponent<EngineManager>(); 
            // engine1을 true로
            engineManager1.engine_1 = true;
            PlayEngineSound();
            engineManager1.CheckAllEngines();
        }
    }

    private void PlayEngineSound()
    {
        if (engineSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(engineSound);
        }
    }    
}
