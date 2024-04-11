using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeColliderScript : MonoBehaviour
{
    public float damage; // Don't need to change, MeleeEnemyPathfinding handles it

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealtScript>().TakeDamage(damage, "Melee");
        }
    }
}
