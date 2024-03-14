using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyPathfinding : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private GameObject player;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    } 

    void Update()
    {
        agent.destination = player.transform.position;
    }
}
