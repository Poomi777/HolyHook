using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSwinginCollider : MonoBehaviour
{
    
    public bool swingAllowed;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Swinging>().isAllowedToSwing = swingAllowed;
        }
    }

}
