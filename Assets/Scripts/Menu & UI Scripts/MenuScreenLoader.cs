using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreenLoader : MonoBehaviour
{
    public bool LevelSelectButton;
    public string loadScene;
    public GameObject MainMenu;
    public GameObject LevelSelect;
    public Scrollbar LevelSelectScrollbar;

    void Start()
    {
        if (LevelSelectButton)
        {
            loadScene = gameObject.transform.parent.GetComponent<DiamondScoreScript>().LevelScoreName;
        }
    }

    public void SeeLevelSelect()
    {
        MainMenu.SetActive(false);
        LevelSelect.SetActive(true);
        LevelSelectScrollbar.value = 1;
    }

    public void BackToMainMenu()
    {
        MainMenu.SetActive(true);
        LevelSelect.SetActive(false);
    }

    public void LoadLevel()
    {
        if (PlayerPrefs.GetInt(loadScene, 0) == 1 || loadScene == "Level1"  || loadScene == "Tutorial"  || loadScene == "PlaytestLevel")
        {
            SceneManager.LoadScene(loadScene);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
