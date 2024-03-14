using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{


    public Transform bulletSpawnPoint;
    public GameObject projectileToFire;
    public float fireRate;
    public float projectileSpeed;

    private float timer = 5f;


    private void Update()
    {
        Fire();
    }
    void Fire()
    {
        fireRate -= Time.deltaTime;
        if (fireRate > 0) return;

        fireRate = timer;

        GameObject projectileObject = Instantiate(projectileToFire, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation) as GameObject;
        Rigidbody projectileRig = projectileObject.GetComponent<Rigidbody>();
        
        projectileRig.AddForce(projectileRig.transform.forward * projectileSpeed);
    }

}
