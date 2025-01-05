using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs; // 생성할 아이템 프리팹 배열
    public Transform[] spawnPoints; // 스폰 지점 배열
    public float spawnInterval = 10f; // 스폰 간격
    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnItem();
            spawnTimer = 0;
        }
    }

    private void SpawnItem()
    {
        if (itemPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        int randomItemIndex = Random.Range(0, itemPrefabs.Length);
        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);

        Instantiate(itemPrefabs[randomItemIndex], spawnPoints[randomSpawnIndex].position, Quaternion.identity);
        Debug.Log("Item spawned: " + itemPrefabs[randomItemIndex].name);
    }
}