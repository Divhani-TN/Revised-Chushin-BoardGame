using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour
{
    public GameObject PauseMenu;

    private void Update()
    {
        // Optional: Add debugging or additional functionality here
    }

    public void LoadAI()
    {
        LoadScene("AI");
    }

    public void LoadMenu()
    {
        LoadScene("Menu");
    }

    public void LoadLost()
    {
        LoadScene("Lost");
    }

    public void LoadRules()
    {
        LoadScene("Rules");
    }

    public void LoadSettings()
    {
        LoadScene("Settings");
    }

    public void LoadWin()
    {
        LoadScene("Win");
    }

    public void LoadGame()
    {
        LoadScene("Game");
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1; // Ensure time scale is reset
        SceneManager.LoadScene(sceneName);
        Debug.Log($"Loading scene: {sceneName}");
    }

    public void Resume()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
        Debug.Log("Game Resumed");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        PauseMenu.SetActive(true);
        Debug.Log("Game Paused");
    }
}

