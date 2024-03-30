using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreenScript : MonoBehaviour
{
    public GameObject EmptyDiamond1;
    public GameObject EmptyDiamond2;
    public GameObject EmptyDiamond3;


    public GameObject FullDiamond1;
    public GameObject FullDiamond2;
    public GameObject FullDiamond3;
    public TextMeshProUGUI KillsText;
    public TextMeshProUGUI TimeText;


    void OnEnable()
    {
        int guarantee = GameManager.instance.SaveScore();
        // im doing this purely so Unity waits a moment before it starts processing info.

        if (guarantee == 1)
        {
            Time.timeScale = 0;
            float TimePassed = GameManager.instance.passedTime;
            int TotalKills = GameManager.instance.enemyKills;
            int DiamondsScored = GameManager.instance.DiamondsAchieved;

            for (int i = 0; i < DiamondsScored; i++)
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

            int minutes = Mathf.FloorToInt(TimePassed / 60);
            int seconds = Mathf.FloorToInt(TimePassed % 60);
            int milliseconds = Mathf.FloorToInt((TimePassed * 1000) % 1000);

            TimeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            KillsText.text = TotalKills.ToString();
        }
    }
}
