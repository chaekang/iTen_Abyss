using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class Enginetwo : MonoBehaviourPun
{
    public void Interact2()
    {
        photonView.RPC("Ex2", RpcTarget.All);
    }

    [PunRPC]
    void Ex2(){
        GameObject engineManagerObject = GameObject.Find("EngineManager");
        EngineManager engineManager2 = engineManagerObject.GetComponent<EngineManager>(); 
        if (engineManager2 != null)
        {
            engineManager2.engine_2 = true;
            engineManager2.CheckAllEngines();
        }
    }
}
