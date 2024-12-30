using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Making : MonoBehaviour
{
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 랜덤 방 참가에 실패하면 새 방을 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
        Debug.Log("create a new Room");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected: " + cause);
        PhotonNetwork.ConnectUsingSettings(); // 접속 실패 시 재접속 시도
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.AutomaticallySyncScene = true;
            //MakeTeams();
        }
        else
        {
            Debug.Log("Not in room..");
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            //PhotonNetwork.LoadLevel("Driving"); //같은 씬을 자동 동기화 함.    
            MakeTeams();
            Invoke("LoadDriving", 2.0f);
        }
    }

    private void LoadDriving()
    {
        PhotonNetwork.LoadLevel("Driving");
    }

    private void MakeTeams()
    {   // 팀 나누기
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Player[] players = PhotonNetwork.PlayerList;
            ExitGames.Client.Photon.Hashtable teamProperties = new ExitGames.Client.Photon.Hashtable();

            for (int i = 0; i < players.Length; i++)
            {
                if (i % 2 == 0)
                { // 짝수 번째 아군
                    teamProperties["Team"] = "Our";
                }
                else
                {   // 홀수 번째 적군
                    teamProperties["Team"] = "Enemy";
                }
                players[i].SetCustomProperties(teamProperties);
                Debug.Log($"플레이어 {players[i].NickName} 팀 배정: {teamProperties["Team"]}");
            }
        }
    }
}