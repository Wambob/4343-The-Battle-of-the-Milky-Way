using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private Entity[] healthPool;
    [SerializeField] private string bossName;

    private float health, maxHealth;

    private void Start()
    {
        //Gather health pool and tell the UI
        foreach (Entity entity in healthPool)
        {
            maxHealth += entity.maxHealth;
            entity.onDamage += UpdateHealth;
        }

        UI.instance.BossHere(name);
    }

    private void UpdateHealth()
    {
        //Update health bar when taking damage
        //If dead, finisht he level
        health = 0;

        foreach (Entity entity in healthPool)
        {
            health += entity.health;
        }

        UI.instance.UpdateBar(UI.BarType.Boss, health / maxHealth);

        if (health <= 0)
        {
            GameplayManager.instance.LevelFinished(true);
        }
    }
}
