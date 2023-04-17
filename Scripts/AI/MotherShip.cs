using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherShip : MonoBehaviour
{
    [SerializeField] private float minTimer, maxTimer;

    private float spawnTimer;

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        //Update timer
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }

        //Periodically spawn enemy ship at position
        if (spawnTimer <= 0)
        {
            ResetTimer();
            GameplayManager.instance.CallForEnemy(transform.position, transform.eulerAngles, GameplayManager.EnemyType.RuntShip);
        }
    }

    private void ResetTimer()
    {
        spawnTimer = Random.Range(minTimer, maxTimer);
    }
}
