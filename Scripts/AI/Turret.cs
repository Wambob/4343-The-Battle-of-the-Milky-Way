using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Entity
{
    #region Variables

    [SerializeField] private Camera cam;
    [SerializeField] private ParticleSystem explosion, fire;
    [SerializeField] private Transform turretBase, turretBarrel;
    [SerializeField] private float blendSpeed, camProximity;

    private GameObject aimTarget;
    private BoxCollider collider;
    private Vector3 adjustedLookAt;

    #endregion

    private void Start()
    {
        health = maxHealth;
        collider = GetComponent<BoxCollider>();
        onDie += Destroyed;
        aimTarget = new GameObject(gameObject.name + " Aim Target");
    }

    private void Update()
    {
        //Update timer
        if (weaponCooldownTimer < equippedWeapon.cooldown)
        {
            weaponCooldownTimer += Time.deltaTime;
        }

        //If turret is destroyed, isn't on screen, or is too far away, deactivate collider
        if (health <= 0 || cam.WorldToScreenPoint(transform.position).y > cam.pixelHeight || Vector3.Distance(transform.position, cam.transform.position) > camProximity)
        {
            collider.enabled = false;
        }
        else if (health > 0)
        {
            collider.enabled = true;

            //Update aim target
            aimTarget.transform.position = Vector3.Lerp(aimTarget.transform.position, GameplayManager.instance.player.transform.position, blendSpeed * Time.deltaTime);
            turretBase.LookAt(aimTarget.transform.position, transform.up);
            adjustedLookAt = turretBase.eulerAngles;
            adjustedLookAt.x = 0;
            adjustedLookAt.z = 0;
            turretBase.transform.eulerAngles = adjustedLookAt;
            turretBarrel.LookAt(aimTarget.transform.position);

            //Shoot Weapon
            if (weaponCooldownTimer >= equippedWeapon.cooldown)
            {
                weaponCooldownTimer = 0;
                Shoot(equippedWeapon, turretBarrel.forward);
            }
        }
    }

    private void Destroyed()
    {
        CameraShake.camShake.AddShake(1, 1);
        aimTarget.transform.parent = gameObject.transform;
        aimTarget.transform.localPosition = new Vector3(0, 0, 1.75f);
        explosion.Play();
        fire.Play();
        collider.enabled = false;
    }
}
