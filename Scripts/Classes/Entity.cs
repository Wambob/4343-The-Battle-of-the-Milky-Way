using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Variables

    public GameplayManager.Team team;
    public WeaponData[] arsenal;
    public WeaponData equippedWeapon;
    public delegate void DieEvent();
    public delegate void DamageEvent();
    public event DieEvent onDie;
    public event DamageEvent onDamage;
    public bool damageEventToggle, isPlayer;
    public float health, maxHealth, weaponCooldownTimer;
    public int score;

    #endregion

    public void ChangeHealth(float value)
    {
        //Subtract from health while keeping between 0 and maximum health
        health = Mathf.Clamp(health + value, 0, maxHealth);

        //If player team, update health bar
        if (team == GameplayManager.Team.Player && isPlayer)
        {
            UI.instance.UpdateBar(UI.BarType.Health, health / maxHealth);

            if (value < 0)
            {
                CameraShake.camShake.AddShake(0.5f, 0.5f);
                AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.HitEffect, true);
            }
        }

        //Call damage event if applicable
        if (damageEventToggle && value < 0)
        {
            onDamage();
        }

        //Call die event if health 0 or below
        if (health <= 0)
        {
            AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.Explosion, true);
            onDie();
        }
    }

    public void Shoot(WeaponData currentWeapon, Vector3 direction)
    {
        AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.Laser, true);
        GameplayManager.instance.CallForProjectile(currentWeapon.shootPoints, currentWeapon.GetFiringDirections(direction, transform.up), currentWeapon.speed, currentWeapon.damage, currentWeapon.moveType, currentWeapon.damageType, team, currentWeapon.objectType);
    }
}

[System.Serializable]
public class WeaponData
{
    #region Variables

    public string name;
    public float speed, cooldown, damage;
    public float[] firingAngleOffset;
    public Transform[] shootPoints;
    public Projectile.MoveType moveType;
    public Projectile.Damagetype damageType;
    public Projectile.ProjectileObject objectType;

    private Vector3[] firingDirections;

    #endregion

    public Vector3[] GetFiringDirections(Vector3 forward, Vector3 up)
    {
        //Transform firing direction based on weapon firing angles by rotating them around an axis
        firingDirections = new Vector3[firingAngleOffset.Length];

        for (int i = 0; i < firingDirections.Length; i += 1)
        {
            firingDirections[i] = Quaternion.AngleAxis(firingAngleOffset[i], up) * forward;
        }

        return firingDirections;
    }
}
