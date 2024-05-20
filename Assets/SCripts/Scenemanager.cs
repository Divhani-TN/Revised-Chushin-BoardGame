using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour
{
    public GameObject PauseMenu;

    private void Update()
    {

    }

    public void LoadAI()
        {
            SceneManager.LoadScene("AI");
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        public void LoadLost()
        {
            SceneManager.LoadScene("Lost");
        }

        public void LoadWin()
        {
            SceneManager.LoadScene("Win");

        }
       public void loadGame()
        {

        SceneManager.LoadScene("Game");


        }

    public void Resume()
    {

        Time.timeScale = 1;
        PauseMenu.SetActive(false);
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        PauseMenu.SetActive(true);
    }
}


