using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MeleeEnemyPathfinding : MonoBehaviour
{
    
    [Header("Attack Settings")]
    public float distanceTilAttack;
    public float attackCooldown;
    public float durationOfAttack;
    public float damage;


    // No touching
    private NavMeshAgent agent;
    private GameObject player;
    private float attackTimer;
    private float durationOfAttackTimer;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
        
        // Attack stuff
        attackTimer = attackCooldown;
        durationOfAttackTimer = -1;

        // Attack Collider Stuff
        gameObject.transform.GetChild(0).gameObject.GetComponent<MeleeColliderScript>().damage = damage;
    } 

    void Update()
    {
        agent.destination = player.transform.position;
        if (Vector3.Distance(transform.position, player.transform.position) <= distanceTilAttack)
        {
            Attack();
        }
    }

    void Attack() // IK this should prob not be in the pathfinding script but good code structure laterrrrr - Ágúst
    {
        if (durationOfAttackTimer >= 0)
        {
            durationOfAttackTimer -= Time.deltaTime;
            if (durationOfAttackTimer < 0)
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(false); // Turning off collider;
                attackTimer = attackCooldown;
            }
        }
        else
        {
            if (attackTimer >= 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(true); // Turning on collider;
                gameObject.GetComponent<Animator>().SetTrigger("Attack");
                durationOfAttackTimer = durationOfAttack;
            }
        }
    }
}
