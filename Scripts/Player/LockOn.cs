using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    private Entity enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Entity>(out enemy) && enemy.team == GameplayManager.Team.Enemy)
        {
            GameplayManager.instance.lockOn = enemy.transform;
        }
    }
}
