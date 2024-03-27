using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool levelComplete;

    public int enemyKills;
    public float passedTime;


    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (!levelComplete)
        {
            passedTime += Time.deltaTime;
        }
    }
}
