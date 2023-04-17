using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private SpawnData[] spawnData;
    [SerializeField] private float minTimer, maxTimer;
    [SerializeField] private int minSpawns, maxSpawns;

    private float spawnTimer;
    private int spawnNumber;

    private void Update()
    {
        //Periodically spawn an enemy in a random location
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            spawnTimer = Random.Range(minTimer, maxTimer);
            spawnNumber = Random.Range(minSpawns, maxSpawns);

            for (int i = 0; i < spawnNumber; i += 1)
            {
                GameplayManager.instance.CallForEnemy(GameplayManager.instance.GetRandomPosition(), transform.eulerAngles, spawnData[Random.Range(0, spawnData.Length)].enemyType);
            }
        }
    }
}

[System.Serializable]
public class SpawnData
{
    public GameplayManager.EnemyType enemyType;
}