using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyPathfinding : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    } 

    void LateUpdate()
    {
        RaycastHit Hit;
        Ray ray = new Ray(transform.position, player.transform.position - transform.position);
        Physics.Raycast(ray, out Hit, Mathf.Infinity);

        if (Hit.collider.gameObject.tag == "Player")
        {
            agent.destination = transform.position;
        }
        else
        {
            agent.destination = player.transform.position;
        }
    }
}
