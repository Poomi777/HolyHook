using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform gunTip, cam, player;
    public LayerMask WhatIsGrappleable;
    public PlayerController2 playerMovement;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse0;

    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;

    private Vector3 currentGrapplePosition;

    //changes
    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;


    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    //

    [Header("SwingingMovement")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;

    [Header("Rope Animation")]
    public int quality = 200; // Number of segments for the rope
    public float damper = 14;
    public float strength = 800;
    public float animationVelocity = 15;
    public float waveCount = 3;
    public float waveHeight = 7;
    public AnimationCurve affectCurve;
    private Spring spring;


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }
    
    void Update()
    {
        MyInput();

        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        CheckForSwingPoints();

        if (joint != null) SwingingMovement();

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }

    }

    void LateUpdate()
    {
        DrawRope();
    }

    private void MyInput()
    {
        //Starting swings or grapples depends on whether or not shift is pressed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(swingKey)) StartGrapple();
        }

        else
        {
            if (Input.GetKeyDown(swingKey)) StartSwing();
        }

        //stopping is always possible
        if (Input.GetKeyUp(swingKey)) StopSwing();
    }

    #region Swinging
    void StartSwing()
    {
        if (predictionHit.point == Vector3.zero)
        {
            return;
        }

        CancelActiveGrapple();
        playerMovement.ResetRestrictions();

        //GetComponent<GrapplingGun2>().StopGrapple();
        //SplayerMovement.ResetRestrictions();
        
        playerMovement.swinging = true;

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        //distance grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;


        //customize these as we like
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lineRenderer.positionCount = 2;
        currentGrapplePosition = gunTip.position;
    }

    public void StopSwing()
    {
        playerMovement.swinging = false;
        lineRenderer.positionCount = 0;
        Destroy(joint);
        //lineRenderer.positionCount = 0;
    }

    // public void StopSwing()
    // {
    //     Debug.Log("StopSwing called");
    //     playerMovement.swinging = false;
    //     if (joint != null)
    //     {
    //         Debug.Log("Destroying joint");
    //         Destroy(joint);
    //         joint = null; // Nullify the joint reference
    //     }
    //     lineRenderer.positionCount = 0;
    // }

    private void SwingingMovement()
    {
        //right
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        //left
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        //forward
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);

        //shorten cable
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

        }

        //extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }

    private void CheckForSwingPoints()
    {
        if (joint != null)
        {
            return;
        }

        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward,
                            out sphereCastHit, maxSwingDistance, WhatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward,
                            out raycastHit, maxSwingDistance, WhatIsGrappleable);        

        Vector3 realHitPoint;

        //option1 Direct Hit
        if (raycastHit.point != Vector3.zero)
        {
            realHitPoint = raycastHit.point;
        }
        //option2 indrect or predicted hit
        else if (sphereCastHit.point != Vector3.zero)
        {
            realHitPoint = sphereCastHit.point;

        }

        else
        {
            realHitPoint = Vector3.zero;

        }

        //realHitPoint found
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;

        }

        //realHitPoint not found
        else
        {
            predictionPoint.gameObject.SetActive(false);
    
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;

    }

    #endregion

    #region grappling

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0)
        {
            return;
        }

        CancelActiveSwing();


        if (predictionHit.point != Vector3.zero)
        {
            swingPoint = predictionHit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }

        else
        {
            swingPoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        //lineRenderer.enabled = true;
        //lineRenderer.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        //the thing that unfreezes player when the grappling is finalyl executed
        //playermovement.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = swingPoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }

        playerMovement.JumpToPosition(swingPoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        //the thing that unfreezes player when the grappling is stopped
        //playermovement.freeze = false;
        playerMovement.ResetRestrictions();

        grapplingCdTimer = grapplingCd;

        //lineRenderer.enabled = false;
    }

    #endregion

    #region  CancelAbilities
    public void CancelActiveGrapple()
    {
        StopGrapple();
    }

    private void CancelActiveSwing()
    {
        StopSwing();
    }


    #endregion

    void DrawRope()
    {
        // if (!joint)
        // {
        //     return;
        // }

        // currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        // lineRenderer.SetPosition(0, gunTip.position);
        // lineRenderer.SetPosition(1, swingPoint);

        if (!joint)
        {
            currentGrapplePosition = gunTip.position;
            spring.Reset();
            if (lineRenderer.positionCount > 0)
                lineRenderer.positionCount = 0;
            return;
        }

        if (lineRenderer.positionCount != quality + 1) {
        lineRenderer.positionCount = quality + 1;
    }

        if (lineRenderer.positionCount == 0)
        {
            spring.SetVelocity(animationVelocity);
            lineRenderer.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 swingDirection = (swingPoint - gunTip.position).normalized;
        Vector3 up = Quaternion.LookRotation(swingDirection) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * animationVelocity);

        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);
            lineRenderer.SetPosition(i, Vector3.Lerp(gunTip.position, currentGrapplePosition, delta) + offset);
        }


    }
}
