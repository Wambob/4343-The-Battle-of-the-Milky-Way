using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAnchor : MonoBehaviour
{
    [SerializeField] private GameObject[] ships;

    private void Start()
    {
        ships[GameData.instance.saveData.shipEquipped].SetActive(true);
    }

    public void Reinitialise()
    {
        //Deactivate all ship objects and reactivate the correct one
        foreach (GameObject ship in ships)
        {
            ship.SetActive(false);
        }

        ships[GameData.instance.saveData.shipEquipped].SetActive(true);
    }
}
