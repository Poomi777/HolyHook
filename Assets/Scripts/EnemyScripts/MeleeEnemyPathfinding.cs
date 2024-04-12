using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MeleeEnemyPathfinding : MonoBehaviour
{
    
    [Header("Attack Settings")]
    public bool turnOff;
    public float distanceTilAttack;
    public float attackCooldown;
    public float durationOfAttack;
    public float damage;
    public float maxHealth;
    public float currentHealth;

    [Header("Grapple Settings")]
    public float standupCooldown;
    public float minObjectHurtSpeed;
    public float objectVelocityMultiplier;
    public float standupYOffset;


    // No touching

    private bool grappled;
    private bool hasBeenGrappled;
    private float standupCooldownTimer;
    private bool standupProtocolInitiated;


    private NavMeshAgent agent;
    private GameObject player;
    private float attackTimer;
    private float durationOfAttackTimer;


    void Start()
    {
        currentHealth = maxHealth;
        standupCooldownTimer = -1;
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
        if (!grappled && !hasBeenGrappled && !turnOff)
        {
            agent.destination = player.transform.position;
        }
        if (Vector3.Distance(transform.position, player.transform.position) <= distanceTilAttack && !turnOff)
        {
            Attack();
        }
        if (standupCooldownTimer >= 0)
        {
            standupCooldownTimer -= Time.deltaTime;
            if (standupCooldownTimer < 0)
            {
                hasBeenGrappled = false;
                standupProtocolInitiated = false;
                agent.enabled = true;
                // May wanna reset rigidbody velocity...
                transform.position = new Vector3(transform.position.x, standupYOffset + transform.position.y, transform.position.z);
                Vector3 direction = (player.transform.position - transform.position).normalized;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // can animate here or add sfx;
        GameManager.instance.enemyKills++;
        StartCoroutine(AudioManager.instance.CreateSoundAtLocation(gameObject.transform.position));
        
        Destroy(this.gameObject);

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

    public void GetGrappled()
    {
        grappled = true;
        agent.enabled = false;
        standupCooldownTimer = -1;
        standupProtocolInitiated = false;
    }

    public void GetReleased()
    {
        grappled = false;
        hasBeenGrappled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasBeenGrappled || grappled)
        {
            if ((other.GetComponent<Rigidbody>().velocity.magnitude > minObjectHurtSpeed || this.GetComponent<Rigidbody>().velocity.magnitude > minObjectHurtSpeed)  && other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                currentHealth -= (other.GetComponent<Rigidbody>().velocity.magnitude + this.GetComponent<Rigidbody>().velocity.magnitude) * objectVelocityMultiplier;
            }
            if (hasBeenGrappled && other.gameObject.layer == LayerMask.NameToLayer("Ground") && !standupProtocolInitiated)
            {
                standupCooldownTimer = standupCooldown;
                standupProtocolInitiated = true;
            }
        }
        else
        {
            if (other.GetComponent<Rigidbody>().velocity.magnitude > minObjectHurtSpeed && other.gameObject.layer != LayerMask.NameToLayer("EnemyProjectile") && other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                currentHealth -= other.GetComponent<Rigidbody>().velocity.magnitude * objectVelocityMultiplier;
            }
        }
    }
}
