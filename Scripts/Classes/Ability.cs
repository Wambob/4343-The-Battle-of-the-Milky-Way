using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public PlayerCharacter player;

    public virtual void InitialiseAbility()
    {
        player = GetComponent<PlayerCharacter>();
        player.onAbility += ActivateAbility;
    }

    public virtual void ActivateAbility()
    {
        
    }
}
