using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public GameObject mainMenuPanel;
    public GameObject pausePanel;
    [Space]
    [Header("Game panel")]
    public GameObject gamePanel;
    public Image healthbar;
    public Text score;

    [Header("Heart Rate panel")]
    public GameObject heartPanel;
    public Text heartRateTxt;
    public Gradient HrColor;

    [Header("Settings panel")]
    public GameObject settingsPanel;
    public Text lowerHRlimit;
    public Text normalHRlimit;
    public Text highHRlimit;
    public Text exerciseDuration;
    public Slider AI;

    void OnEnable()
    {
        GameflowChanger.Instance.RaiseEvent += OnGameFlowUpdate;
    }

    void Start()
    {
        mainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
        gamePanel.SetActive(false);
        heartPanel.SetActive(false);
    }

    void OnGameFlowUpdate(object sender, GameEventArgs e)
    {
        //changing heart rate color 
		
        heartRateTxt.color = HrColor.Evaluate(e.delayRate);
    }
    public float color = 0.1f;

    void Update()
    {
        if (GameController.Instance.player == null) return;
        if (MyUduinoManager.Instance.IsConnected)
            UpdateHeartRateText(MyUduinoManager.Instance.HR);

        SetHealthBar();
    }

    public void SetHealthBar()
    {
        healthbar.fillAmount = (float)GameController.Instance.player.Health / SettingsManager.Instance.playersetting.maxhealth;
    }

    public void UpdateHeartRateText(int hr)
    {
        heartRateTxt.text = hr.ToString();
    }

    public void GamePaused(bool isGamePaused)
    {
        if (isGamePaused)
        {
            pausePanel.SetActive(true);
        }
        else pausePanel.SetActive(false);
    }
    #region Unity UI Btn Events
    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(true);
        heartPanel.SetActive(true);
        GameController.Instance.StartGame();
    }

    public void OpenSettingsMenu()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    //called by save btn in settings
    public void SaveSettings()
    {
        try
        {
            SettingsManager.Instance.SaveSettings(
              Convert.ToInt32(lowerHRlimit.text),
              Convert.ToInt32(normalHRlimit.text),
              Convert.ToInt32(highHRlimit.text),
              (int)AI.value,
              Convert.ToInt32(exerciseDuration.text)
                );
        }
        catch (FormatException)
        {
            RestoreDefaults();
            print("Input in Incorrect Format");
        }
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    //called by restore btn in settings
    public void RestoreDefaults()
    {
        SettingsManager.Instance.RestoreDefaults();
        lowerHRlimit.text = SettingsManager.Instance.lowerHRlimit.ToString();
        normalHRlimit.text = SettingsManager.Instance.normalHRlimit.ToString();
        highHRlimit.text = SettingsManager.Instance.highHRlimit.ToString();
        exerciseDuration.text = SettingsManager.Instance.exerciseDuration.ToString();
    }

    public void Quit()
    {
        GameController.Instance.Quit();
    }
    //called in pause screen menu
    public void QuitToMainMenu()
    {
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        GameController.Instance.QuitToMainMenu();
    }
    #endregion
}