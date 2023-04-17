using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float speed;

    private PlayerCharacter player;

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if triggered by player
        if (other.TryGetComponent<PlayerCharacter>(out player))
        {
            AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.PowerUp, true);
            Debug.Log(gameObject.name);
            player.StartPowerUp();
            gameObject.SetActive(false);
        }
    }
}
