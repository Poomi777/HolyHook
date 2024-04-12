using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreenLoader : MonoBehaviour
{
    public GameObject TutorialDiamonds;
    public GameObject Level1Diamonds;
    public GameObject Level2Diamonds;
    public GameObject Level3Diamonds;
    public GameObject Level4Diamonds;
    public GameObject PlaytestDiamonds;



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
    
    public void ResetScores()
    {
        PlayerPrefs.SetInt("Level1", 0);
        PlayerPrefs.SetInt("Level2", 0);
        PlayerPrefs.SetInt("Level3", 0);
        PlayerPrefs.SetInt("Level4", 0);
        PlayerPrefs.SetInt("Tutorial", 0);
        PlayerPrefs.SetInt("PlaytestLevel", 0);
        PlayerPrefs.SetInt("Level2Unlock", 0);
        PlayerPrefs.SetInt("Level3Unlock", 0);
        PlayerPrefs.SetInt("Level4Unlock", 0);
        PlayerPrefs.Save();

        TutorialDiamonds.GetComponent<DiamondScoreScript>().UpdateDiamonds();
        Level1Diamonds.GetComponent<DiamondScoreScript>().UpdateDiamonds();
        Level2Diamonds.GetComponent<DiamondScoreScript>().UpdateDiamonds();
        Level3Diamonds.GetComponent<DiamondScoreScript>().UpdateDiamonds();
        Level4Diamonds.GetComponent<DiamondScoreScript>().UpdateDiamonds();
        PlaytestDiamonds.GetComponent<DiamondScoreScript>().UpdateDiamonds();
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
        string unlock = loadScene + "Unlock";
        if (PlayerPrefs.GetInt(unlock, 0) == 1 || loadScene == "Level1"  || loadScene == "Tutorial"  || loadScene == "PlaytestLevel")
        {
            SceneManager.LoadScene(loadScene);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
