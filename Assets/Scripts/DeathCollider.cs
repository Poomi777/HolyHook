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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer ("Object") || other.gameObject.layer == LayerMask.NameToLayer ( "Enemy" ))
        {
            Destroy(other.gameObject);
        }
    }
}
