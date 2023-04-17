using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : Ship
{
    [SerializeField] private float blendSpeed;

    private GameObject target;
    private Vector3 moveDirection;

    private void OnEnable()
    {
        health = maxHealth;
    }

    private void Start()
    {
        health = maxHealth;
        target = new GameObject(gameObject.name + "Target");
        target.transform.parent = GameplayManager.instance.parent.transform;
        target.transform.localPosition = GameplayManager.instance.GetRandomPosition();
        onDie += Deactivate;
    }

    private void Update()
    {
        //Update timers
        if (weaponCooldownTimer < equippedWeapon.cooldown)
        {
            weaponCooldownTimer += Time.deltaTime;
        }

        //Move, like an enemy ship
        if (Vector3.Distance(transform.position, target.transform.position) <= 1)
        {
            target.transform.localPosition = GameplayManager.instance.GetRandomPosition();
            //Set target to be alongside the player so the drones 
            target.transform.localPosition = new Vector3(target.transform.localPosition.x, target.transform.localPosition.y, GameplayManager.instance.player.transform.localPosition.z);
        }

        moveDirection = Vector3.Lerp(moveDirection, (target.transform.localPosition - transform.localPosition).normalized, blendSpeed * Time.deltaTime);
        SetRoll(moveDirection.x, moveDirection.z);

        transform.localPosition += moveDirection * speed * Time.deltaTime;
        transform.localPosition = GameplayManager.instance.GetValidPosition(transform.localPosition);

        //Shoot
        if (weaponCooldownTimer > equippedWeapon.cooldown && GameplayManager.instance.lockOn != null)
        {
            Shoot(equippedWeapon, transform.forward);
            weaponCooldownTimer = 0;
        }
    }

    private void Deactivate()
    {
        health = maxHealth;
        GameplayManager.instance.CallForExplosion(transform.position);
        gameObject.SetActive(false);
    }
}
