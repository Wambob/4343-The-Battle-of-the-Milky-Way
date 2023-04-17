using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public SaveData saveData;
    public int sceneToLoad;

    private BinaryFormatter binaryFormatter;
    private FileStream stream;
    private string path;

    private void Awake()
    {
        //Make this a singleton
        //Important to destroy duplicate as this gameobject persists between scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Cannot serialize 2 dimensional arrays so must do it in awake
        saveData.upgradesBought = new bool[3, saveData.shipsBought.Length];

        //Look for a persistant data path that can be accessed between game sessions
        binaryFormatter = new BinaryFormatter();
        path = Application.persistentDataPath + ("/savedata.mkw");
        LoadData();
    }

    public void SaveData()
    {
        //Feed savedata to the binary formatter
        //Close the stream when done
        stream = new FileStream(path, FileMode.Create);
        binaryFormatter.Serialize(stream, saveData);
        stream.Close();
    }

    public void LoadData()
    {
        //Load data if it exists
        //If no data exists, won't load data and player effectively starts a new game
        //Close stream afterwards regardless
        if (File.Exists(path))
        {
            Debug.Log(path);
            stream = new FileStream(path, FileMode.Open);
            saveData = binaryFormatter.Deserialize(stream) as SaveData;
        }
        stream.Close();
    }
}

[System.Serializable]
public class SaveData
{
    public bool[] shipsBought;
    public bool[,] upgradesBought;
    public bool tutorialComplete;
    public int highScore, spaceBucks, shipEquipped;
    public float shakeIntensity, volumeSFX, volumeMusic;
}
