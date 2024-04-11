using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestLevelWinScript : MonoBehaviour
{

    public bool hasTriggered;

    void OnTriggerEnter()
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            GameManager.instance.dontSaveScore = true;
            GameManager.instance.DiamondsAchieved++;
            
            if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0) < GameManager.instance.DiamondsAchieved)
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, GameManager.instance.DiamondsAchieved);
            }

            if (GameManager.instance.DiamondsAchieved == 3)
            {
                GameObject.Find("Player").GetComponent<PlayerController>().WinTrigger();
            }
        }
    }
}
