using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private bool isWireConnected = false;

    public void ConnectWire()
    {
        if (!isWireConnected)
        {
            isWireConnected = true;
            Debug.Log("전선이 엔진에 연결되었습니다.");
            // 엔진 구동 시작 코드 추가
        }
        else
        {
            Debug.Log("이미 전선이 연결되어 있습니다.");
        }
    }
}