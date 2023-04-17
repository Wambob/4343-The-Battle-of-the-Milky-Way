using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollAbility : Ability
{
    [SerializeField] private float rollDistance, rollDuration;

    private Vector2 rollVector;

    public override void InitialiseAbility()
    {
        base.InitialiseAbility();

        //Apply upgrade data
        if (GameData.instance.saveData.upgradesBought[2, GameData.instance.saveData.shipEquipped])
        {
            rollDistance *= 1 + player.upgradePercent;
        }
    }

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        //Apply an instant transformation on the player if ability cooldown is finished
        //Roll ship object too
        if (player.abilityTimer <= 0)
        {
            player.abilityTimer = player.abilityCooldown;

            CameraShake.camShake.AddShake(0.5f, 0.5f);

            rollVector = player.moveInput.normalized * rollDistance;

            transform.position = transform.position + transform.forward * rollVector.y + transform.right * rollVector.x;

            StartCoroutine(player.Roll(rollDuration, Mathf.Sign(player.moveInput.x)));
        }
    }
}
