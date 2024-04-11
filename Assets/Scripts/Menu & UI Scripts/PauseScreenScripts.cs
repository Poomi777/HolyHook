using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenScripts : MonoBehaviour
{

    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    public void ReloadScene()
    {
        player.GetComponent<PlayerController>().PauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNewScene()
    {
        player.GetComponent<PlayerController>().PauseGame();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Main Menu");
    }

    public void UnpauseGame()
    {
        player.GetComponent<PlayerController>().PauseGame();
    }
}
