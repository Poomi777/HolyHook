using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScreenLoader : MonoBehaviour
{
    public string loadScene;

    public void LoadNewScene()
    {
        SceneManager.LoadScene(loadScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UnpauseGame()
    {
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().PauseGame();
    }
}
