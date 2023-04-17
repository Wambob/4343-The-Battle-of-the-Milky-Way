using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMP : MonoBehaviour
{
    private EnemyShip enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyShip>(out enemy) && enemy.team == GameplayManager.Team.Enemy)
        {
            enemy.EMP();
        }
    }
}
