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
    
    public string nextLevel;
    public float minTime;
    public float maxTime;
    public float minKills;
    public float maxKills;
    public float barrierForOneStar; // from 0 to 200;
    public float barrierFortwoStars; // from 0 to 200;
    public float barrierForthreeStars; // from 0 to 200;
    public int DiamondsAchieved;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        passedTime += Time.deltaTime;
    }

    public int SaveScore()
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
                    if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0) < 3)
                    {
                        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 3);
                    }
                    DiamondsAchieved = 3;
                }
                else
                {
                    if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0) < 2)
                    {
                        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 2);
                    }
                    DiamondsAchieved = 2;
                }
            }
            else
            {
                if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0) < 1)
                {
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
                }                
                DiamondsAchieved = 1;
            }
        }
        else
        {
            if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0) <= 0)
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 0);
            }       
            DiamondsAchieved = 0;
        } 

        PlayerPrefs.SetInt(nextLevel, 1);
        PlayerPrefs.Save();
        return 1;
    }
}
