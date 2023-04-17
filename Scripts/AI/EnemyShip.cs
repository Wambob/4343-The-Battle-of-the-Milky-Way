using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : Ship
{
    #region Variables

    [SerializeField] private float blendSpeed, shootRange, powerUpChance, stunDuration;

    private GameObject target;
    private Vector3 moveDirection;
    private IEnumerator stunIE;
    private bool stunned;

    #endregion

    private void Start()
    {
        health = maxHealth;
        target = new GameObject(gameObject.name + "Target");
        onDie += Dead;
        target.transform.parent = GameplayManager.instance.parent.transform;
        target.transform.localPosition = GameplayManager.instance.GetRandomPosition();
    }

    private void Update()
    {
        if (!stunned)
        {
            //Update timers
            if (weaponCooldownTimer < equippedWeapon.cooldown)
            {
                weaponCooldownTimer += Time.deltaTime;
            }

            //Move
            //Find a new position for movement target when reached
            if (Vector3.Distance(transform.position, target.transform.position) <= 1)
            {
                target.transform.localPosition = GameplayManager.instance.GetRandomPosition();
            }

            //Lerps movement direction to create the illusion of weight
            moveDirection = Vector3.Lerp(moveDirection, (target.transform.localPosition - transform.localPosition).normalized, blendSpeed * Time.deltaTime);
            SetRoll(-moveDirection.x, -moveDirection.z);

            //Apply transformation while staying in bounds
            transform.localPosition += moveDirection * speed * Time.deltaTime;
            transform.localPosition = GameplayManager.instance.GetValidPosition(transform.localPosition);

            //Shoot weapon if player is in sights and weapon has cooled down
            if (weaponCooldownTimer > equippedWeapon.cooldown && Mathf.Abs(transform.localPosition.x - GameplayManager.instance.player.transform.localPosition.x) <= shootRange)
            {
                Shoot(equippedWeapon, (GameplayManager.instance.player.transform.position - transform.position).normalized);
                weaponCooldownTimer = 0;
            }
        }
        else
        {
            //When stunned, only move as normal but at one tenth speed
            moveDirection = Vector3.Lerp(moveDirection, (target.transform.localPosition - transform.localPosition).normalized, blendSpeed * 0.1f * Time.deltaTime);
            SetRoll(-moveDirection.x, -moveDirection.z);

            transform.localPosition += moveDirection * speed * 0.1f * Time.deltaTime;
            transform.localPosition = GameplayManager.instance.GetValidPosition(transform.localPosition);
        }
    }

    private void Dead()
    {
        if (Random.Range(0f, 100f) <= powerUpChance)
        {
            GameplayManager.instance.CallForPowerUp(transform.position, transform.forward);
        }
        UI.instance.AddScore(score);
        CameraShake.camShake.AddShake(1, 1);
        GameplayManager.instance.currentEnemies -= 1;
        GameplayManager.instance.CallForExplosion(transform.position);
        Destroy(target);
        Destroy(gameObject);
    }

    public void EMP()
    {
        //Caching the coroutine prevents potential errors from calling the coroutine multiple times
        if (stunIE != null)
        {
            StopCoroutine(stunIE);
        }
        stunIE = Stun();
        StartCoroutine(stunIE);
    }

    private IEnumerator Stun()
    {
        stunned = true;
        yield return new WaitForSeconds(stunDuration);
        stunned = false;
    }
}