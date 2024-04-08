using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondScoreScript : MonoBehaviour
{

    public string LevelScoreName;
    public GameObject EmptyDiamond1;
    public GameObject EmptyDiamond2;
    public GameObject EmptyDiamond3;


    public GameObject FullDiamond1;
    public GameObject FullDiamond2;
    public GameObject FullDiamond3;

    void Start()
    {
        int number_of_diamonds = PlayerPrefs.GetInt(LevelScoreName, 0);

        for (int i = 0; i < number_of_diamonds; i++)
        {
            if (i == 1)
            {
                FullDiamond1.SetActive(true);
            }
            else if (i == 2)
            {
                FullDiamond2.SetActive(true);
            }
            else if (i == 3)
            {
                FullDiamond3.SetActive(true);
            }
        }
    }
}
