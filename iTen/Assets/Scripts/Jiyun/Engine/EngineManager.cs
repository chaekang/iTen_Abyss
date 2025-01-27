using UnityEngine;
using Photon.Pun;
using TMPro;

public class EngineManager : MonoBehaviourPun
{
    public bool engine_1;
    public bool engine_2;
    public bool engine_3;
    public TextMeshProUGUI allClearText; // "All Clear" 텍스트를 표시할 TextMeshProUGUI 컴포넌트
    public EndingCutscene cutsceneManager;

    public void CheckAllEngines()
    {
        
        // 모든 엔진이 true인가
        if (engine_1 && engine_2 && engine_3)
        {
            photonView.RPC("PlayCutsceneRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void PlayCutsceneRPC() 
    {
        StartCoroutine(cutsceneManager.PlayCutscene()); // 컷씬 실행
    }
}
