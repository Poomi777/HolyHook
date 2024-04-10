using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassWinObject : MonoBehaviour
{
    private GameObject player;
    public AudioSource Glass;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Glass.Play();
            player.GetComponent<PlayerController>().WinTrigger();
        }
    }
}
