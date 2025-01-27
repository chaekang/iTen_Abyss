using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameScene : MonoBehaviourPunCallbacks
{   // Loading씬에서 Game씬으로
    public override void OnDisconnected(DisconnectCause cause){
        Debug.LogError("Disconnected: " + cause);
        PhotonNetwork.ConnectUsingSettings(); // 접속 실패 시 재접속 시도
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2){    
            PhotonNetwork.LoadLevel("Map");
        }
    }
}
