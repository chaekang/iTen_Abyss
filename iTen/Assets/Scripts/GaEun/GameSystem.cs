using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    public bool IsFlashlightOn { get; private set; }
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

    public void ToggleFlashlight()
    {
        IsFlashlightOn = !IsFlashlightOn;

        if (IsFlashlightOn)
        {
            Debug.Log("손전등 켰다.");
        }
        else
        {
            Debug.Log("손전등 껐다");
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
