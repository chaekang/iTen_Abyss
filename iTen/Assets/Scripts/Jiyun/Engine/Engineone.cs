using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Engineone : MonoBehaviourPun
{
    public TextMeshProUGUI clearText; // 플레이어들이 오브젝트 누를 시 텍스트 띄움
    public float pressTime = 10f;   // 마우스를 10초 이상 눌러야 함
    private List<Photon.Realtime.Player> pressingPlayers = new List<Photon.Realtime.Player>();
    // 각 플레이어의 마우스 누르기 시작하는 시간
    private Dictionary<Photon.Realtime.Player, float> pressStartTime = new Dictionary<Photon.Realtime.Player, float>();
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
            // 텍스트 표시
            photonView.RPC("ShowClearText", RpcTarget.All);

            engineManager1.CheckAllEngines();
        }
    }

    [PunRPC]
    void ShowClearText()
    {
        //clearText.text = "Clear!";
    }

    
    /*public void OnMouseUp()
    {
        photonView.RPC("StopPressing", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void StartPressing(Photon.Realtime.Player player)
    {
        // 마우스를 누르고 있는 플레이어 목록에 추가
        if (!pressingPlayers.Contains(player))
        {
            pressingPlayers.Add(player);
            pressStartTime[player] = Time.time; // 누르기 시작 시간 저장
            StartCoroutine(CheckPressTime(player)); // 시간 측정 코루틴 시작
        }
    }

    [PunRPC]
    void StopPressing(Photon.Realtime.Player player)
    {
        // 마우스를 누르고 있는 플레이어 목록에서 제거합니다.
        if (pressingPlayers.Contains(player))
        {
            pressingPlayers.Remove(player);
            pressStartTime.Remove(player); // 누르기 시작 시간 제거
        }
    }

    IEnumerator CheckPressTime(Photon.Realtime.Player player)
    {
        // 마우스를 누르고 있는 시간을 측정
        while (pressingPlayers.Contains(player))
        {
            float elapsedTime = Time.time - pressStartTime[player];
            if (elapsedTime >= pressTime && pressingPlayers.Count == 2)
            {
                // 두 명의 플레이어가 10초 이상 누르고 있으면 텍스트 표시
                photonView.RPC("ShowClearText", RpcTarget.All);
                break; // 코루틴 종료
            }
            yield return null; // 다음 프레임까지 대기
        }
    }*/
    
}
