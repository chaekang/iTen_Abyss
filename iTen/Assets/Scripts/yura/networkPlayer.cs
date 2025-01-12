using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class networkPlayer : MonoBehaviourPunCallbacks
{
    

    // Start is called before the first frame update
    void Start()
    {
        

        MonoBehaviour[] Scripts = GetComponents<MonoBehaviour>();

        for(int i = 0; i < Scripts.Length; i++)
        {
            if (Scripts[i] is networkPlayer) continue;
            else if (Scripts[i] is PhotonView) continue;

            Scripts[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
