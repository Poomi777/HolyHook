using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("References")]

    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrapplable;
    public LineRenderer lineRenderer;

    private PlayerController playerMovement;
    

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        playerMovement = GetComponent<PlayerController>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) 
        {
            StartGrapple();
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }

    }

    private void LateUpdate()
    {
        if(grappling)
        {
            lineRenderer.SetPosition(0, gunTip.position);
        }
    }


    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) 
        {
            return;
        }

        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrapplable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }

        else 
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {

    }

    private void StopGrapple()
    {
        grappling = false;

        grapplingCdTimer = grapplingCd;

        lineRenderer.enabled = false;
    }
}
