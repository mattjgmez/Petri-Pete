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
    // Unused currently
    //public Canvas MainCanvas;
    //public GameObject HUD;

    public Image HealthSegment;
    public Image DamageSegment;

    public GameObject PauseScreen;
    public GameObject DeathScreen;
    public GameObject VictoryScreen;
    public GameObject CreditsScreen;

    public TMP_Text PointsText;

    public GameObject EventLocationMarker;

    public GameObject LogMessagePrefab;
    public Transform LogbookContentTransform;

    protected virtual void Start()
    {
        SetPauseScreen(false);
        SetDeathScreen(false);
        SetVictoryScreen(false);
    }

    #region PUBLIC METHODS

    #region CORE METHODS

    /// <summary>
    /// Loads the main menu via GameManager.
    /// </summary>
    public void LoadMainMenu()
    {
        GameManager.Instance.LoadScene("MainMenu");
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
        if (PauseScreen != null && PauseManager.Instance != null)
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
    /// Sets the victory screen on or off.
    /// </summary>
    public virtual void SetCreditsScreen(bool state)
    {
        if (CreditsScreen != null)
        {
            CreditsScreen.SetActive(state);
        }
    }

    public virtual void LoadSceneInt(int sceneBuildIndex)
    {
        GameManager.Instance.LoadScene(sceneBuildIndex);
    }

    #endregion

    public virtual void ShowSpawnOnMinimap(Vector2 location)
    {
        //if (EventLocationMarker == null) { return; }

        //Animator anim = EventLocationMarker.GetComponent<Animator>();

        //if (anim != null)
        //{
        //    anim.SetTrigger("MarkEvent") ;
        //}

        //EventLocationMarker.transform.position = location;
    }

    /// <summary>
    /// Updates the health bar based on current, min, and max health values.
    /// </summary>
    /// <param name="currentHealth">Current health value.</param>
    /// <param name="minHealth">Minimum possible health.</param>
    /// <param name="maxHealth">Maximum possible health.</param>
    public virtual void UpdateHealthBar(float healthPercentage)
    {
        if (HealthSegment == null) { return; }
        if (DamageSegment == null) { return; }

        Debug.Log($"{this.GetType()}.UpdateHealthBar: Health % before clamp = {healthPercentage}.", gameObject);

        healthPercentage = Mathf.Clamp01(healthPercentage);  // Ensure it's between 0 and 1

        Debug.Log($"{this.GetType()}.UpdateHealthBar: Health % after clamp = {healthPercentage}.", gameObject);

        HealthSegment.fillAmount = healthPercentage;

        float damageTaken = 1.0f - healthPercentage;
        DamageSegment.fillAmount = damageTaken;
    }

    public void AddJournalEntryWithID(string ID)
    {
        string logMessage = JournalEntries.GetEntry(ID);
        if (logMessage == "" || logMessage == null) { return; }
        AddLogMessage(logMessage);
    }

    /// <summary>
    /// Creates a LogMessagePrefab as a child of the Logbook.
    /// </summary>
    /// <param name="message">The message to be entered as text.</param>
    public void AddLogMessage(string message)
    {
        if (LogMessagePrefab == null) { return; }
        if (LogbookContentTransform == null) { return; }

        GameObject newLog = Instantiate(LogMessagePrefab, LogbookContentTransform);
        newLog.GetComponent<TMP_Text>().text = message;

        // Scroll to the bottom
        Canvas.ForceUpdateCanvases(); // Update the Canvas immediately
        LogbookContentTransform.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
    }

    public void SetPoints(int points)
    {
        if (PointsText == null) { return; }
        PointsText.text = $"Points: {points.ToString("N0")}";
    }

    #endregion
}
