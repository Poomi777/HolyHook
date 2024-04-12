using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealtScript>().TakeDamage(99999, "Melee");
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer ("Object") || other.gameObject.layer == LayerMask.NameToLayer ( "Enemy" ))
        {
            Destroy(other.gameObject);
            if (other.gameObject.layer == LayerMask.NameToLayer ( "Enemy" ))
            {
                GameManager.instance.enemyKills++;
            }
        }
    }
}
