using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject[] projectileObjects;
    [SerializeField] private ParticleSystem explosionPtcl;
    [SerializeField] private float blendspeed;

    public enum MoveType { None, Linear, Log, Cosine, LockOn };
    public enum Damagetype { None, Direct, Explosive }
    public enum ProjectileObject { Laser = 0, PlayerLaser = 1, Missile = 2, Mine = 3 };

    private MoveType moveType;
    private Damagetype dmgType;
    private GameplayManager.Team team;
    private Entity target;
    private Animator anim;
    private Explosion explosionScript;
    private Transform lockOn;
    private float lifetime, progress, speed, damage;

    #endregion

    private void Start()
    {
        anim = GetComponent<Animator>();
        explosionScript = explosionPtcl.GetComponent<Explosion>();
    }

    private void Update()
    {
        //Update lifetime timer
        lifetime -= Time.deltaTime;
        progress += Time.deltaTime;

        //Deactivate when lifetime reaches 0
        if (lifetime <= 0)
        {
            gameObject.SetActive(false);
        }

        //Move the projectile based on it's MoveType
        switch (moveType)
        {
            case (MoveType.Linear):
                transform.position += transform.forward * speed * Time.deltaTime;
                break;
            case (MoveType.Log):
                transform.position += transform.forward * (2 * Mathf.Log10(progress) + 2) * speed * Time.deltaTime;
                break;
            case (MoveType.Cosine):
                transform.position += (transform.forward + transform.parent.right * Mathf.Cos(Time.time * 7)) * speed * Time.deltaTime;
                break;
            case (MoveType.LockOn):
                //Change direction based on lockOn target
                if (lockOn != null)
                {
                    transform.forward = Vector3.Lerp(transform.forward, (lockOn.position - transform.position).normalized, blendspeed * Time.deltaTime);
                }
                transform.position += transform.forward * speed * Time.deltaTime;
                break;
        }
    }

    public void PrepProjectile(Vector3 direction, float speedSet, float damageSet, MoveType moveTypeSet, Damagetype dmgTypeSet, GameplayManager.Team teamSet, ProjectileObject objectSet, float lifetimeSet)
    {
        //Stop particle effects
        explosionPtcl.Stop();
        explosionPtcl.Clear();

        //Set variables
        transform.forward = direction;
        speed = speedSet;
        damage = damageSet;
        moveType = moveTypeSet;
        dmgType = dmgTypeSet;
        team = teamSet;
        lifetime = lifetimeSet;
        progress = 0;

        //Start at random rotation if it's using lockon movetype
        //Lockon target is determined here so different projectiles can have different targets
        if (moveType == MoveType.LockOn)
        {
            transform.forward = Quaternion.AngleAxis(Random.Range(-30f, 30f), transform.parent.up) * transform.forward;

            if (GameplayManager.instance.lockOn != null)
            {
                lockOn = GameplayManager.instance.lockOn;
            }
        }

        //Use appropriate projectile object
        HideObjects();
        projectileObjects[((int)objectSet)].SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if an entity on a different team was hit
        if (other.TryGetComponent<Entity>(out target) && target.team != team)
        {
            //Effect depends on damagetype
            switch (dmgType)
            {
                case (Damagetype.Direct):
                    target.ChangeHealth(-damage);
                    lifetime = 0;
                    break;
                case (Damagetype.Explosive):
                    HideObjects();
                    anim.SetTrigger("Explode");
                    explosionPtcl.Stop();
                    explosionPtcl.Play();
                    explosionScript.SetExplode(damage, team);
                    lifetime = 5;
                    moveType = MoveType.None;
                    dmgType = Damagetype.None;
                    AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.Explosion, true);
                    break;
            }
        }
    }

    private void HideObjects()
    {
        foreach (GameObject projectile in projectileObjects)
        {
            projectile.SetActive(false);
        }
    }
}