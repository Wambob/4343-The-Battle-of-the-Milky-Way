using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class UI : MonoBehaviour
{
    #region Variables

    [SerializeField] private Image[] bars;
    [SerializeField] private TextMeshProUGUI scoreText, cumulativeScoreText, multiplierText, notificationText, bossNameText, timerText;
    [SerializeField] private GameObject[] menus;
    [SerializeField] private GameObject bossStats;
    [SerializeField] private float comboCooldown, shakeRange, shakeSpeed;
    [SerializeField] private int maxMultiplier;

    public enum BarType { Health = 0, Ability = 1, Combo = 2, Boss = 3 };

    public static UI instance;

    private Player uiInputs;
    private InputAction pause;
    private Animator anim;
    private Vector2 multiplierPos;
    private IEnumerator comboIE;
    private float comboTimer;
    private int score, cumulativeScore, multiplier;

    #endregion

    private void Awake()
    {
        instance = this;

        Cursor.lockState = CursorLockMode.Locked;

        uiInputs = new Player();

        pause = uiInputs.Ship.Pause;

        pause.performed += OnPause;
    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        //Pause/unpause the game by using time.timescale
        if (Time.timeScale == 1)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            menus[0].SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            menus[0].SetActive(false);
            menus[1].SetActive(false);
        }
    }

    private void OnEnable()
    {
        pause.Enable();
    }

    private void OnDisable()
    {
        pause.Disable();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        multiplierPos = multiplierText.rectTransform.localPosition;
    }

    private void Update()
    {
        //Update combo timer
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            UpdateBar(BarType.Combo, comboTimer / comboCooldown);
            multiplierText.rectTransform.localPosition = multiplierPos + new Vector2(shakeRange * Mathf.Cos(Time.time * shakeSpeed * 3.7f * multiplier), shakeRange * Mathf.Cos(Time.time * shakeSpeed * multiplier));
        }
    }

    #region Public Functions

    public void UpdateBar(BarType typeSelect, float amount)
    {
        bars[((int)typeSelect)].fillAmount = amount;
    }

    public void AddScore(int amount)
    {
        //Increase multiplier and increase cumulative score
        //Cumulative score is a temporary score that applies to the real score after the combo
        multiplier = Mathf.Clamp(multiplier + 1, 0, maxMultiplier);
        cumulativeScore += amount;
        if (comboIE != null)
        {
            StopCoroutine(comboIE);
        }
        comboIE = StartCombo();
        StartCoroutine(comboIE);
    }

    public void CallForNotification(string text)
    {
        notificationText.text = text;
        anim.SetTrigger("Notify");
    }

    public void BossHere(string name)
    {
        bossStats.SetActive(true);
        bossNameText.text = name;
    }

    public void TimerUpdate(float timeRemaining)
    {
        timerText.text = ("Survive for " + timeRemaining + "s");
    }

    public void LoadScene(int index)
    {
        //Update highscore is applicable
        //Increase spacebucks, adding on cumulative score too
        //Go to loading screen where gamedata holds the index of the scene to be loaded
        if (score + cumulativeScore * multiplier > GameData.instance.saveData.highScore)
        {
            GameData.instance.saveData.highScore = score + cumulativeScore * multiplier;
        }
        GameData.instance.saveData.spaceBucks += score + cumulativeScore * multiplier;
        GameData.instance.sceneToLoad = index;
        Time.timeScale = 1;
        SceneManager.LoadScene("Loading");
    }
    
    public void LoadMenu(int menuID)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }

        menus[menuID].SetActive(true);
    }

    public void ButtonSound()
    {
        AudioManager.singletonInstance.PlayAudio(AudioManager.AudioInstance.Button, false);
    }

    public void DisableInputs()
    {
        pause.Disable();
    }

    #endregion

    private IEnumerator StartCombo()
    {
        //Display multiplier text
        //Reset combo cooldown
        //When combo ends, apply cumulative score and display real score
        comboTimer = comboCooldown;
        cumulativeScoreText.text = ("+" + cumulativeScore);
        multiplierText.text = ("x" + multiplier + "!");
        yield return new WaitForSeconds(comboCooldown);
        score += cumulativeScore * multiplier;
        cumulativeScore = 0;
        multiplier = 0;
        scoreText.text = ("Score: " + score);
        cumulativeScoreText.text = (" ");
        multiplierText.text = (" ");
        UpdateBar(BarType.Combo, 0);
    }
}