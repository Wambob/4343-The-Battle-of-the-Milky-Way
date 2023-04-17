using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private float decaySpeed, maxRange, maxSpeed, disparity, range, speed;

    public static CameraShake camShake;

    private Vector3 shake;

    private void Start()
    {
        //Load shake intensity setting
        camShake = this;
        maxRange *= GameData.instance.saveData.shakeIntensity;
        maxSpeed *= GameData.instance.saveData.shakeIntensity;
    }

    private void Update()
    {
        //Update range of shake and speed
        range -= decaySpeed * Time.deltaTime;
        speed -= decaySpeed * Time.deltaTime;

        range = Mathf.Clamp(range, 0, maxRange);
        speed = Mathf.Clamp(speed, 0, maxSpeed);

        //Apply transformation to camera
        cameraObject.transform.position = transform.position + ShakeVector();
    }

    public void AddShake(float addRange, float addSpeed)
    {
        range += addRange * maxRange;
        speed += addSpeed * maxSpeed;
    }

    private Vector3 ShakeVector()
    {
        //Use Cos function and a disparity between x and y axis movement to create the illusion of a "random" shaking pattern
        shake = (cameraObject.transform.up * Mathf.Cos(Time.time * speed) * range) + (cameraObject.transform.right * Mathf.Cos(Time.time * speed * disparity) * range);

        return shake;
    }
}