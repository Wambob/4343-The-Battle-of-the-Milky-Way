using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float timerSet;
    [SerializeField] private int decimalPoints;

    private float timer;
    private bool cutOff;

    private void Start()
    {
        timer = timerSet;
    }

    private void Update()
    {
        //Each frame update UI timer
        //When timer is finished, complete the level
        timer -= Time.deltaTime;

        if (timer < 0 && !cutOff)
        {
            GameplayManager.instance.LevelFinished(true);
            cutOff = true;
        }
        else
        {
            UI.instance.TimerUpdate(Mathf.Round(Mathf.Clamp(timer, 0, timerSet) * Mathf.Pow(10, decimalPoints)) / Mathf.Pow(10, decimalPoints));
        }
    }
}
