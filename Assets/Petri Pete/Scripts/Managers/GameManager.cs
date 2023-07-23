using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    public int CurrentSceneIndex = 0;

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialization();
    }

    protected virtual void Initialization()
    {
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public virtual void NextScene()
    {
        CurrentSceneIndex++;

        if (CurrentSceneIndex > SceneManager.sceneCountInBuildSettings)
        {
            CurrentSceneIndex = 0;
        }

        SceneManager.LoadScene(CurrentSceneIndex);
    }

    public virtual void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public virtual void RestartRun()
    {
        SceneManager.LoadScene("Floor1");
    }

    public virtual void CloseGame()
    {
        Application.Quit();
    }

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
}
