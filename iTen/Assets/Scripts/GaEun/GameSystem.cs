using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }


    public bool IsSafeZone { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }



    public void ToggleSafeZone()
    {
        IsSafeZone = !IsSafeZone;

        if (IsSafeZone)
        {
            Debug.Log("안전한 곳에 들어왔다.");
        }
        else
        {
            Debug.Log("안전한 곳에 나갔다.");
        }
    }
}
