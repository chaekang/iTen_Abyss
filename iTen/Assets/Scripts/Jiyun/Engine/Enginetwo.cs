using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class Enginetwo : MonoBehaviourPun
{
    public int wire_count = 0;
    public void Interact2()
    {
        photonView.RPC("Ex2", RpcTarget.All);
        Debug.Log("wire count: " + wire_count);
    }

    [PunRPC]
    void Ex2(){
        GameObject engineManagerObject = GameObject.Find("EngineManager");
        EngineManager engineManager2 = engineManagerObject.GetComponent<EngineManager>(); 

        wire_count++;
        if (engineManager2 != null)
        {
            if(wire_count == 2){
                engineManager2.engine_2 = true;
                engineManager2.CheckAllEngines();
            }
        }
    }
}
