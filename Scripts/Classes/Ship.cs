using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : Entity
{
    public GameObject[] boosters;
    public GameObject shipObject;

    public float maxRoll, speed;

    private float rollPolarity, timer, timerDuration, originalRoll;
    private bool rolling;

    public void SetRoll(float xValue, float yValue)
    {
        //Set rotation of ship object
        //Perform barrel roll if rolling
        //Update scale of jets to show movement
        if (!rolling)
        {
            shipObject.transform.localRotation = Quaternion.Lerp(shipObject.transform.localRotation, Quaternion.Euler(0, 0, -maxRoll * xValue), 10 * Time.deltaTime);
        }
        else
        {
            timer -= Time.deltaTime;
            shipObject.transform.localRotation = Quaternion.Lerp(shipObject.transform.localRotation, Quaternion.Euler(0, 0, originalRoll + 360 * rollPolarity * (timer / timerDuration)), 10 * Time.deltaTime);
        }

        foreach (GameObject jet in boosters)
        {
            jet.transform.localScale = new Vector3(1 - 0.2f * yValue, 1 + 0.5f * yValue, 1 - 0.2f * yValue);
        }
    }

    public IEnumerator Roll(float duration, float polarity)
    {
        rolling = true;
        originalRoll = shipObject.transform.eulerAngles.z;
        rollPolarity = polarity;
        timerDuration = duration;
        timer = duration;
        yield return new WaitForSeconds(duration);
        rolling = false;
    }
}