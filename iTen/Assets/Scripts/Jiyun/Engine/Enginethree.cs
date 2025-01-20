using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class Enginethree : MonoBehaviourPun
{   // 배터리3개
    public void Interact3()
    {
        photonView.RPC("Ex3", RpcTarget.All);
    }

    [PunRPC]
    void Ex3(){
        GameObject engineManagerObject = GameObject.Find("EngineManager");
        EngineManager engineManager3 = engineManagerObject.GetComponent<EngineManager>();
        if (engineManager3 != null)
        {
            engineManager3.engine_3 = true;
            engineManager3.CheckAllEngines();
        }
    }
}
