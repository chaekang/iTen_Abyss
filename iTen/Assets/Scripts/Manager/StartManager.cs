using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    // 이 매니저는 싱글톤이 아닌 단순히 카메라와 플레이어를 연결하는 역할을 하는 매니저
    // StartManager라고 안해도 되니 상관 없음

    [SerializeField] private CinemachineVirtualCamera _camera;

    private void Start()
    {
        // 플로우 단계
        // 1. 포톤 환경 설정 및 입장 다 하기

        // 2. 캐릭터 스폰
        SpawnManager spawnMawnager = SpawnManager.Instance;
        spawnMawnager.spawn.SpawnPlayer();

        // 3. 카메라와 연결 시켜주기
        _camera.Follow = spawnMawnager.spawn.player.GetComponent<FirstPersonController>().FollowTransform;
    }
}
