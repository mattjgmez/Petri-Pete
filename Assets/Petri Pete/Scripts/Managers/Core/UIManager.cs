using JadePhoenix.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles all UI effects and changes.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    public Canvas MainCanvas;
    public GameObject HUD;
    public Image HealthBar;
    public TextMeshProUGUI HealthText;
    public GameObject PauseScreen;
    public GameObject DeathScreen;
    public GameObject VictoryScreen;
    public TMP_Text PointsText;
    public TMP_Text DebuffsText;
    public TMP_Text UpgradeTimer;
    public TMP_Text RemainingEnemies;
    public GameObject UpgradeSelectScreen;

    protected virtual void Start()
    {
        SetPauseScreen(false);
        SetDeathScreen(false);
        SetVictoryScreen(false);
    }

    #region PUBLIC METHODS

    /// <summary>
    /// Loads the main menu via GameManager.
    /// </summary>
    public void LoadMainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }

    /// <summary>
    /// Exits the game via GameManager.
    /// </summary>
    public void CloseGame()
    {
        GameManager.Instance.CloseGame();
    }

    /// <summary>
    /// Sets the pause screen on or off.
    /// </summary>
    public virtual void SetPauseScreen(bool state)
    {
        if (PauseScreen != null)
        {
            PauseScreen.SetActive(state);
            PauseManager.Instance.SetPause(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the death screen on or off.
    /// </summary>
    public virtual void SetDeathScreen(bool state)
    {
        if (DeathScreen != null)
        {
            DeathScreen.SetActive(state);
            PauseManager.Instance.SetPause(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Sets the victory screen on or off.
    /// </summary>
    public virtual void SetVictoryScreen(bool state)
    {
        if (VictoryScreen != null)
        {
            VictoryScreen.SetActive(state);
            PauseManager.Instance.SetPause(state);
            EventSystem.current.sendNavigationEvents = state;
        }
    }

    /// <summary>
    /// Updates the health bar based on current, min, and max health values.
    /// </summary>
    /// <param name="currentHealth">Current health value.</param>
    /// <param name="minHealth">Minimum possible health.</param>
    /// <param name="maxHealth">Maximum possible health.</param>
    public virtual void UpdateHealthBar(float currentHealth, float minHealth, float maxHealth)
    {
        if (HealthBar == null) { return; }

        float newHealth = currentHealth / (maxHealth);

        HealthBar.fillAmount = newHealth;
        HealthText.text = currentHealth.ToString();
    }

    public virtual void UpdateDebuffs(List<Debuff> debuffs)
    {
        if (DebuffsText == null) { return; }

        string newText = debuffs.Count == 0 ? "None" : "";
        foreach (var item in debuffs)
        {
            newText += item.name + " ";
        }
        DebuffsText.text = newText;

    }

    /// <summary>
    /// Updates the displayed upgrade timer.
    /// </summary>
    /// <param name="time">Time remaining for the next upgrade.</param>
    public virtual void UpdateUpgradeTimer(float time)
    {
        if (UpgradeTimer == null) { return; }

        if (time >= 60)
        {
            int minutes = (int)time / 60;
            UpgradeTimer.text = minutes.ToString("D") + "m";
        }
        else
        {
            UpgradeTimer.text = ((int)time).ToString();
        }
    }

    #endregion
}
