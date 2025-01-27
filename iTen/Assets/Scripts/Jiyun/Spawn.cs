using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using UnityEngine;

public class Spawn : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public GameObject player;
    [SerializeField] private CinemachineVirtualCamera _camera;

    void Start()
    {
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.SendRate = 30;

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint(0);

        if (spawnPoint != null)
        {
            GameObject player = PhotonNetwork.Instantiate("PlayerCapsule", spawnPoint.position, spawnPoint.rotation, 0);
            //player = Instantiate(player, spawnPoint.position, spawnPoint.rotation);
            _camera.Follow = player.GetComponent<FirstPersonController>().FollowTransform;
        }
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateAllPlayersSpawn", RpcTarget.All);
        }
    }

    [PunRPC]
    void UpdateAllPlayersSpawn()
    {
        SpawnPlayer();
    }
    

}
