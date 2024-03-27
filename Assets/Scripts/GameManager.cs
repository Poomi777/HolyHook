using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Current Score")]
    public int enemyKills;
    public float passedTime;


    [Header("Final Score Settings")]
    public float minTime;
    public float maxTime;
    public float minKills;
    public float maxKills;
    public float barrierForOneStar; // from 0 to 200;
    public float barrierFortwoStars; // from 0 to 200;
    public float barrierForthreeStars; // from 0 to 200;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        passedTime += Time.deltaTime;
    }

    public void SaveScore()
    {
        float tempKillScore; // from 0 to 100
        float tempTimeScore; // from 0 to 100
        
        if (passedTime > maxTime)
        {
            tempTimeScore = 0;
        }
        else
        {
            tempTimeScore = (1 - ((passedTime - minTime) / (maxTime - minTime))) * 100;
        }
        if (enemyKills < minKills)
        {
            tempKillScore = 0;
        }
        else
        {
            tempKillScore = enemyKills / maxKills * 100;
        }

        float tempTotalScore = tempKillScore + tempTimeScore;

        if (tempTotalScore >= barrierForOneStar)
        {
            if (tempTotalScore >= barrierFortwoStars)
            {
                if (tempTotalScore >= barrierForthreeStars)
                {        
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 3);
                }
                else
                {
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 2);
                }
            }
            else
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
            }
        }
        else
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 0);
        } 
    }
}
