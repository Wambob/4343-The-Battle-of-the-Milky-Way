using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hint, loadingText;
    [SerializeField] private Image loadingBar;
    [TextArea]
    [SerializeField] private string[] hintList;

    private AsyncOperation sceneToLoad;
    private bool continueBool;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        hint.text = hintList[Random.Range(0, hintList.Length)];
        StartCoroutine(LoadSelectedScene());
    }

    private IEnumerator LoadSelectedScene()
    {
        //Start scene load as an async operation and don't allow scene to load yet
        sceneToLoad = SceneManager.LoadSceneAsync(GameData.instance.sceneToLoad);
        sceneToLoad.allowSceneActivation = false;
        sceneToLoad.completed += CompleteLoad;

        //Wait for player confirmation
        while (!continueBool)
        {
            //Update text when loading finished
            //The last 10% is finished upon switching scene
            if (sceneToLoad.progress >= 0.9f)
            {
                loadingText.text = ("Loading finished, click anywhere to continue");
            }
            loadingBar.fillAmount = sceneToLoad.progress / 0.9f;
            yield return null;
        }

        sceneToLoad.allowSceneActivation = true;
    }

    private void CompleteLoad(AsyncOperation obj)
    {
        Debug.Log("Finished");
        AudioManager.singletonInstance.StopMusic();
        AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.LevelMusic, false);
        GameData.instance.SaveData();
    }

    public void Continue()
    {
        if (sceneToLoad.progress >= 0.9f)
        {
            continueBool = true;
        }
    }
}
