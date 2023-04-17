using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject[] menus;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform[] anchors;
    [SerializeField] private Slider shakeBar, sfxBar, musicBar;
    [SerializeField] private TextMeshProUGUI highScoreText, saleText, boughtText, shipStats, shipAbility;
    [SerializeField] private TextMeshProUGUI[] upgradeDescription, upgradeBought, currencyText;
    [SerializeField] private CatalogueData[] catalogue;
    [SerializeField] private Image thumbnail;
    [SerializeField] private ShipAnchor[] shipAnchors;
    [SerializeField] private float blendSpeed;

    public int anchorPointer;

    private int settingID, shipID;

    #endregion

    private void Start()
    {
        //Play menu music
        AudioManager.singletonInstance.StopMusic();
        AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.MenuMusic, false);

        //Load data from savedata
        shakeBar.value = GameData.instance.saveData.shakeIntensity;
        sfxBar.value = GameData.instance.saveData.volumeSFX;
        musicBar.value = GameData.instance.saveData.volumeMusic;
        highScoreText.text = ("High Score: " + GameData.instance.saveData.highScore);
        UpdateCurrency();
        UpdateUpgradeDescription();
        shipID = GameData.instance.saveData.shipEquipped - 1;
        ChangeShip(true);

        Cursor.lockState = CursorLockMode.None;

    }

    private void Update()
    {
        //Update camera transform based on current menu
        cam.position = Vector3.Lerp(cam.position, anchors[anchorPointer].position, blendSpeed * Time.deltaTime);
        cam.rotation = Quaternion.Lerp(cam.rotation, anchors[anchorPointer].rotation, blendSpeed * Time.deltaTime);
    }

    #region Public Functions

    public void ChangeMenu(int menuID)
    {
        for (int i = 0; i < menus.Length; i += 1)
        {
            if (i != menuID)
            {
                menus[i].SetActive(false);
            }
            else
            {
                menus[i].SetActive(true);
            }
        }

        anchorPointer = menuID;
    }

    public void UpdateShip()
    {
        foreach (ShipAnchor anchor in shipAnchors)
        {
            anchor.Reinitialise();
        }
    }

    public void ChangeScene(int sceneID)
    {
        GameData.instance.sceneToLoad = sceneID;
        SceneManager.LoadScene("Loading");
    }

    public void UpdateVolume()
    {
        AudioManager.singletonInstance.UpdateVolumes();
    }

    public void SettingID(int value)
    {
        settingID = value;
    }

    public void ChangeSetting(Slider currentSlider)
    {
        switch (settingID)
        {
            case (0):
                GameData.instance.saveData.shakeIntensity = currentSlider.value;
                break;
            case (1):
                GameData.instance.saveData.volumeSFX = currentSlider.value;
                break;
            case (2):
                GameData.instance.saveData.volumeMusic = currentSlider.value;
                break;
        }
    }

    public void BuyShip()
    {
        //If player can afford ship, buy it and update savedata
        //Otherwise, if player owns ship, change to ship upgrade menu
        if (!GameData.instance.saveData.shipsBought[shipID] && GameData.instance.saveData.spaceBucks >= catalogue[shipID].price)
        {
            GameData.instance.saveData.spaceBucks -= catalogue[shipID].price;
            GameData.instance.saveData.shipsBought[shipID] = true;
            GameData.instance.saveData.shipEquipped = shipID;
            saleText.text = ("Equipped: The " + catalogue[shipID].name);
            boughtText.text = ("Upgrade");
            UpdateCurrency();
        }
        else if (GameData.instance.saveData.shipsBought[shipID])
        {
            UpdateUpgradeDescription();
            ChangeMenu(4);
        }
    }

    public void BuyUpgrade(int ID)
    {
        //If player can afford upgrade, buy it and update savedata
        if (GameData.instance.saveData.shipsBought[shipID] && !GameData.instance.saveData.upgradesBought[ID, shipID] && GameData.instance.saveData.spaceBucks >= catalogue[shipID].upgradePrice[ID])
        {
            GameData.instance.saveData.spaceBucks -= catalogue[shipID].upgradePrice[ID];
            GameData.instance.saveData.upgradesBought[ID, shipID] = true;
            upgradeBought[ID].text = ("Bought");
            UpdateCurrency();
            UpdateUpgradeDescription();
        }
    }

    public void ChangeShip(bool next)
    {
        //Loop shipID around if necessary
        if (next)
        {
            shipID += 1;
            if (shipID >= GameData.instance.saveData.shipsBought.Length)
            {
                shipID = 0;
            }
        }
        else
        {
            shipID -= 1;
            if (shipID < 0)
            {
                shipID = GameData.instance.saveData.shipsBought.Length - 1;
            }
        }

        //Update tumbnail and descriptions
        thumbnail.sprite = catalogue[shipID].thumbnail;
        shipStats.text = (catalogue[shipID].stats);
        shipAbility.text = (catalogue[shipID].ability);

        //Update ship equipped if player owns current ship
        if (GameData.instance.saveData.shipsBought[shipID])
        {
            GameData.instance.saveData.shipEquipped = shipID;
            saleText.text = ("Equipped: The " + catalogue[shipID].name);
            boughtText.text = ("Upgrade");
        }
        else
        {
            saleText.text = ("For sale: The " + catalogue[shipID].name + " ($B" + catalogue[shipID].price + ")");
            boughtText.text = ("Buy");
        }
    }

    public void ButtonSound()
    {
        AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.Button, false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    private void UpdateCurrency()
    {
        foreach (TextMeshProUGUI text in currencyText)
        {
            text.text = ("$B: " + GameData.instance.saveData.spaceBucks);
        }
    }

    private void UpdateUpgradeDescription()
    {
        for (int i = 0; i < upgradeDescription.Length; i += 1)
        {
            upgradeDescription[i].text = (catalogue[shipID].upgradeDescription[i] + " $B" + catalogue[shipID].upgradePrice[i]);
            if (GameData.instance.saveData.upgradesBought[i, shipID])
            {
                upgradeBought[i].text = ("Bought");
            }
            else
            {
                upgradeBought[i].text = ("Buy");
            }
        }
    }
}

[System.Serializable]
public class CatalogueData
{
    public Sprite thumbnail;
    public string name;
    [TextArea]
    public string stats, ability;
    public int price;
    [TextArea]
    public string[] upgradeDescription;
    public int[] upgradePrice;
}
