using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAbility : Ability
{
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private int squadAmount;

    private GameObject[] droneSquad;
    private IEnumerator recallIE;

    public override void InitialiseAbility()
    {
        base.InitialiseAbility();

        //Apply upgrade data
        if (GameData.instance.saveData.upgradesBought[2, GameData.instance.saveData.shipEquipped])
        {
            squadAmount += 2;
        }

        //Initialise drone squad
        droneSquad = new GameObject[squadAmount];

        //Spawn drones
        for (int i = 0; i < droneSquad.Length; i += 1)
        {
            droneSquad[i] = Instantiate(dronePrefab, GameplayManager.instance.parent.transform);
            droneSquad[i].SetActive(false);
        }
    }

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        //Activate all drone objects and give them random positions
        if (player.abilityTimer <= 0)
        {
            player.abilityTimer = player.abilityCooldown;

            for (int i = 0; i < droneSquad.Length; i += 1)
            {
                droneSquad[i].SetActive(true);
                droneSquad[i].transform.localPosition = GameplayManager.instance.GetRandomPosition();
            }

            if (recallIE != null)
            {
                StopCoroutine(recallIE);
            }
            recallIE = RecallSquad();
            StartCoroutine(recallIE);
        }
    }

    private IEnumerator RecallSquad()
    {
        //Deactivate drones after a given amount of time
        yield return new WaitForSeconds(player.abilityCooldown / 2);

        for (int i = 0; i < droneSquad.Length; i += 1)
        {
            droneSquad[i].SetActive(false);
        }
    }
}
