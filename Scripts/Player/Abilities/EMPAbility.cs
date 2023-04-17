using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPAbility : Ability
{
    [SerializeField] private ParticleSystem blast;

    private Animator anim;

    public override void InitialiseAbility()
    {
        base.InitialiseAbility();

        //Get animator component and apply upgrade data
        anim = GetComponent<Animator>();
        if (GameData.instance.saveData.upgradesBought[2, GameData.instance.saveData.shipEquipped])
        {
            player.abilityCooldown *= 1 - player.upgradePercent;
        }
    }

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        //Play EMP animation after clearing particles
        if (player.abilityTimer <= 0)
        {
            player.abilityTimer = player.abilityCooldown;

            blast.Stop();
            blast.Clear();
            blast.Play();
            anim.SetTrigger("EMP");
        }
    }
}
