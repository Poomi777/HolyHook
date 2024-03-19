using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour
{

    public Vector3 StartPosition;
    public GameObject Player;

    void Start()
    {
        StartPosition = Player.transform.position;
    }

    void OnTriggerEnter()
    {
        Player.transform.position = StartPosition;
    }
}
