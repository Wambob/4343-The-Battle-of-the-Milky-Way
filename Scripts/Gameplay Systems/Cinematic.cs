using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cinematic : MonoBehaviour
{
    public void CinematicEnd()
    {
        SceneManager.LoadScene(1);
    }
}
