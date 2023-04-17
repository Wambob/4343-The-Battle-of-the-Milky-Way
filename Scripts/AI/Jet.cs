using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jet : Entity
{
    [SerializeField] private Camera cam;
    [SerializeField] private ParticleSystem stream, explosion, fire;

    private BoxCollider collider;

    private void Start()
    {
        health = maxHealth;
        collider = GetComponent<BoxCollider>();
        onDie += Destroyed;
    }

    private void Update()
    {
        //If jet is destroyed or is off screen, deactivate collider
        if (health <= 0 || cam.WorldToScreenPoint(transform.position).y > cam.pixelHeight)
        {
            collider.enabled = false;
        }
        else if (health > 0)
        {
            collider.enabled = true;
        }
    }

    private void Destroyed()
    {
        CameraShake.camShake.AddShake(1, 1);
        stream.Stop();
        explosion.Play();
        fire.Play();
        collider.enabled = false;
    }
}
