using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private GameplayManager.Team team;
    private Entity target;
    private float damage;

    public void SetExplode(float damageSet, GameplayManager.Team teamSet)
    {
        damage = damageSet;
        team = teamSet;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Damage enemies in explosion radius
        if (other.TryGetComponent<Entity>(out target) && target.team != team)
        {
            target.ChangeHealth(-damage);
        }
    }
}
