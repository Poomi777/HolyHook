using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    private GameObject Player;
    private Transform target;
    public float homingSpeed = 5f;
    public float bulletLifeTime;
    public float damage;



    private Rigidbody projectileRigidBody;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        target = Player.transform;
        projectileRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        if (target != null && bulletLifeTime > 0)
        {
            Vector3 targetDirection = (target.position - transform.position).normalized;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, homingSpeed * Time.fixedDeltaTime, 0f);
            projectileRigidBody.rotation = Quaternion.LookRotation(newDirection);

            bulletLifeTime -= Time.fixedDeltaTime;
            projectileRigidBody.velocity = transform.forward * homingSpeed;
        }
        else
        {
            Destroy(this.gameObject); // Destroy the specific instnace, not all instances
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealtScript>().TakeDamage(damage, "Ranged");
        }
        if (other.gameObject.layer != LayerMask.NameToLayer("EnemyProjectile"))
        {
            Destroy(this.gameObject); // Destroy the specific instnace, not all instances
        }
    }
}
