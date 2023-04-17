using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverclockAbility : Ability
{
    [SerializeField] private float multiplier;

    private IEnumerator overclockIE;
    private float upgradePercent;

    public override void InitialiseAbility()
    {
        base.InitialiseAbility();

        if (GameData.instance.saveData.upgradesBought[2, GameData.instance.saveData.shipEquipped])
        {
            upgradePercent = player.upgradePercent;
        }
    }

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        //Start coroutine if ability cooldown is finished
        if (player.abilityTimer <= 0)
        {
            player.abilityTimer = player.abilityCooldown;

            if (overclockIE != null)
            {
                StopCoroutine(overclockIE);
            }
            overclockIE = Overclock();
            StartCoroutine(overclockIE);
        }
    }

    private IEnumerator Overclock()
    {
        //Alter damage, cooldown, and speed of each weapon in arsenal for a given amount of time
        foreach (WeaponData weapon in player.arsenal)
        {
            weapon.damage *= 1 + multiplier;
            weapon.cooldown *= 1 - multiplier;
            weapon.speed *= 1 + upgradePercent;
        }
        yield return new WaitForSeconds(player.abilityCooldown / 4);
        foreach (WeaponData weapon in player.arsenal)
        {
            weapon.damage /= 1 + multiplier;
            weapon.cooldown /= 1 - multiplier;
            weapon.speed /= 1 + upgradePercent;
        }
    }
}
