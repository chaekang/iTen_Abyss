using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;

    public static SpawnManager Instance { get; private set; }

    public Spawn spawn;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform GetSpawnPoint(int index)
    {
        if (index >= 0 && index < spawnPoints.Length)
        {
            return spawnPoints[index];
        }
        return null;
    }
}
