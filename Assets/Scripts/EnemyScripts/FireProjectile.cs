using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{


    public Transform bulletSpawnPoint;
    public GameObject projectileToFire;
    private GameObject player;
    public float fireRate;
    public float projectileSpeed;

    private float timer = 5f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        RaycastHit Hit;
        Ray ray = new Ray(transform.position, player.transform.position - transform.position);
        Physics.Raycast(ray, out Hit, Mathf.Infinity);

        if (Hit.collider.gameObject.tag == "Player")
        {
            Fire();
        }

    }
    void Fire()
    {
        if(timer > 0) { return; }

        GameObject projectileObject = Instantiate(projectileToFire, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation) as GameObject;
        
        timer = fireRate;
    }

}
