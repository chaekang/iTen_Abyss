using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Waiting : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown){
            SceneManager.LoadScene("Lobby");
            Debug.Log("방 만들기로 전환");
        }
    }
}
