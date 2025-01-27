using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;

public class Enginethree : MonoBehaviourPun
{   // 배터리3개
    public int battery_count = 0;

    public AudioClip engineSound;
    private AudioSource audioSource;

    public void Interact3()
    {
        photonView.RPC("Ex3", RpcTarget.All);
    }

    [PunRPC]
    void Ex3(){
        GameObject engineManagerObject = GameObject.Find("EngineManager");
        EngineManager engineManager3 = engineManagerObject.GetComponent<EngineManager>();

        battery_count++;
        if (engineManager3 != null)
        {
            if(battery_count == 3){
                PlayEngineSound();
                engineManager3.engine_3 = true;
                engineManager3.CheckAllEngines();
            }
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
