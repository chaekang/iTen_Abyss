using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Debug.Log("������ ���� ���Դ�.");
        }
        else
        {
            Debug.Log("������ ���� ������.");
        }
    }

    public void OnClickStart()
    {
        Invoke("LoadScene", 1f);
    }
    private void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}
