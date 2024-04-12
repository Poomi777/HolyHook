using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondScoreScript : MonoBehaviour
{

    public GameObject Lock;
    public string LevelScoreName;
    public GameObject EmptyDiamond1;
    public GameObject EmptyDiamond2;
    public GameObject EmptyDiamond3;


    public GameObject FullDiamond1;
    public GameObject FullDiamond2;
    public GameObject FullDiamond3;

    void Start()
    {
        int unlocked = PlayerPrefs.GetInt(LevelScoreName + "Unlock", 0);
        if (unlocked == 1 || LevelScoreName == "Level1"  || LevelScoreName == "Tutorial"  || LevelScoreName == "PlaytestLevel")
        {
            Lock.SetActive(false);
        }

        int number_of_diamonds = PlayerPrefs.GetInt(LevelScoreName, 0);

        for (int i = 0; i < number_of_diamonds; i++)
        {
            if (i == 0)
            {
                FullDiamond1.SetActive(true);
            }
            else if (i == 1)
            {
                FullDiamond2.SetActive(true);
            }
            else if (i == 2)
            {
                FullDiamond3.SetActive(true);
            }
        }
    }

    public void UpdateDiamonds()
    {
        Lock.SetActive(true);

        int unlocked = PlayerPrefs.GetInt(LevelScoreName + "Unlock", 0);
        if (unlocked == 1 || LevelScoreName == "Level1"  || LevelScoreName == "Tutorial"  || LevelScoreName == "PlaytestLevel")
        {
            Lock.SetActive(false);
        }

        FullDiamond1.SetActive(false);
        FullDiamond2.SetActive(false);
        FullDiamond3.SetActive(false);

        int number_of_diamonds = PlayerPrefs.GetInt(LevelScoreName, 0);

        for (int i = 0; i < number_of_diamonds; i++)
        {
            if (i == 0)
            {
                FullDiamond1.SetActive(true);
            }
            else if (i == 1)
            {
                FullDiamond2.SetActive(true);
            }
            else if (i == 2)
            {
                FullDiamond3.SetActive(true);
            }
        }
    }
}
