using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyPathfinding : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;

    [Header("Attack Settings")]
    public float maxHealth;
    public float currentHealth;

    [Header("Grapple Settings")]
    public float standupCooldown;
    public float minObjectHurtSpeed;
    public float objectVelocityMultiplier;
    public float standupYOffset;
    public bool canShoot;
    [SerializeField]
    public Animator rangedAnimation;

    // No touching

    private bool grappled;
    private bool hasBeenGrappled;
    private float standupCooldownTimer;
    private bool standupProtocolInitiated;


    void Start()
    {
        currentHealth = maxHealth;
        standupCooldownTimer = -1;
        canShoot = true;

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    } 

    void LateUpdate()
    {
        if (!grappled && !hasBeenGrappled)
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
            rangedAnimation.SetBool("IsWalking", agent.velocity.magnitude > 0.01f);
        }
        rangedAnimation.SetBool("IsWalking", false);
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
                canShoot = true;
                
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
        Destroy(this.gameObject);
    }

    public void GetGrappled()
    {
        canShoot = false;
        hasBeenGrappled = false;
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

            if (other.GetComponent<Rigidbody>().velocity.magnitude > minObjectHurtSpeed || this.GetComponent<Rigidbody>().velocity.magnitude > minObjectHurtSpeed)
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
            if (other.GetComponent<Rigidbody>().velocity.magnitude > minObjectHurtSpeed && other.gameObject.layer != LayerMask.NameToLayer("EnemyProjectile"))
            {
                currentHealth -= other.GetComponent<Rigidbody>().velocity.magnitude * objectVelocityMultiplier;
            }
        }
    }
}
