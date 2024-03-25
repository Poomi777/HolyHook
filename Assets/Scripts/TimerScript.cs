using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private float currentTime = 0f; 
    private TextMeshProUGUI mainTimer; 
    private TextMeshProUGUI mainTimerBackground;

    void Start()
    {
        mainTimer = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        mainTimerBackground = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        UpdateTimerDisplay();
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        int milliseconds = Mathf.FloorToInt((currentTime * 1000) % 1000);

        mainTimer.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        mainTimerBackground.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
