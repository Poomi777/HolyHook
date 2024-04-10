using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{


    public Transform bulletSpawnPoint;
    public GameObject projectileToFire;
    public RangedEnemyPathfinding pathfinding;
    private GameObject player;
    public float fireRate;
    public float projectileSpeed;

    public Animator rangedAnimation;

    private float timer = 0f;

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

        if (Hit.collider.gameObject.tag == "Player" && pathfinding.canShoot)
        {
            Fire();
        }

    }

    void FireProjectileTimedAnimation()
    {
        Instantiate(projectileToFire, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);

        timer = fireRate;
    }

    void Fire()
    {
        if(timer > 0) { return; }
        rangedAnimation.SetTrigger("FirePojectileAnimation");
    }


}
