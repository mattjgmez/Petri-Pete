using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game's overall state and scene transitions.
/// </summary>
public class GameManager : PersistentSingleton<GameManager>
{
    // Current scene's index in the build settings.
    public int CurrentSceneIndex = 0;

    protected virtual void OnEnable()
    {
        // Subscribe to the scene loaded event.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnDisable()
    {
        // Unsubscribe from the scene loaded event.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called when a new scene is loaded.
    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialization();
    }

    // Initialize necessary components or settings after a scene is loaded.
    protected virtual void Initialization()
    {
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    #region PUBLIC METHODS

    /// <summary>
    /// Load the next scene in the build settings.
    /// </summary>
    public virtual void NextScene()
    {
        CurrentSceneIndex++;

        if (CurrentSceneIndex > SceneManager.sceneCountInBuildSettings)
        {
            CurrentSceneIndex = 0;
        }

        SceneManager.LoadScene(CurrentSceneIndex);
    }

    /// <summary>
    /// Load the main menu scene.
    /// </summary>
    public virtual void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quit the application.
    /// </summary>
    public virtual void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Trigger game over behavior, displaying either victory or defeat UI.
    /// </summary>
    /// <param name="victory">If true, trigger victory. Otherwise, trigger defeat.</param>
    public virtual void TriggerGameOver(bool victory)
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.SetPause(true);
            PauseManager.Instance.GameOver = true;
        }

        if (UIManager.Instance == null) { return; }

        if (victory)
        {
            UIManager.Instance.SetVictoryScreen(true);
        }
        else
        {
            UIManager.Instance.SetDeathScreen(true);
        }
    }

    #endregion
}
