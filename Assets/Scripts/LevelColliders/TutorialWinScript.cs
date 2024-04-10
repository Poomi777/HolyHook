using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialWinScript : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.dontSaveScore = true;
            GameManager.instance.DiamondsAchieved = 3;

            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, GameManager.instance.DiamondsAchieved);

            player.GetComponent<PlayerController>().WinTrigger();
            PlayerPrefs.Save();
        
        }
    }
}
