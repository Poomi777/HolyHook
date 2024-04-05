using StarterAssets;
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
    public PlayerController playerMovement;

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

    private bool isGrappleActive = false;


    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    //

    [Header("SwingingMovement")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float shortenCableSpeed;
    public float extendCableSpeed;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;
    public GameObject CrosshairIndicator;

    [Header("Rope Animation")]
    public int quality = 200; // Number of segments for the rope
    public float damper = 14;
    public float strength = 800;
    public float animationVelocity = 15;
    public float waveCount = 3;
    public float waveHeight = 7;
    public AnimationCurve affectCurve;
    private Spring spring;

    // For new input system
    private StarterAssetsInputs _input;
    private bool isSwinging = false;

    [Header("Audio")]
    public AudioClip[] swingSounds; // Array of swing sounds.
    private AudioSource audioSource;

    [Header("Object Grappling")]
    public KeyCode grappleObjectKey = KeyCode.Mouse1; //Right-click by default
    public Transform grappledObject; //current grappled object
    private SpringJoint objectJoint; //spring joint used for grappling objects
    public LayerMask whatIsGrappleObject; //layer mask defining which objects can be grappled
    public float objectGrappleSpring = 4.5f;
    public float objectGrappleDamper = 7f;
    public float objectGrappleMassScale = 0.1f;
    public float objectConnectedMassScale = 0.1f;
    public float maxObjectGrappleDistance = 25f; //maximum distance to grapple an object

    public bool isObjectGrappleActive = false;

    public float swingAngleToJump = 90.0f;
    
    Vector3 posAtStartSwing;


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);

        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        MyInput();

        CheckForSwingPoints();

        if (joint != null) SwingingMovement();

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }

        if (objectJoint != null)
        {
            DragGrappleObject();
        }
        

    }

    void LateUpdate()
    {
        DrawRope();

        if (grappledObject)
        {
            AnimateGrappleObject(grappledObject);
        }
    }

    private void MyInput()
    {
        //Starting swings or grapples depends on whether or not shift is pressed

        if (_input.sprint && !isSwinging)
        {
            if (_input.swing && grapplingCdTimer <= 0f)
            {
                StartGrapple();
                isSwinging = true;
                grapplingCdTimer = grapplingCd;
            }
        }

        else
        {
            if (_input.swing && !isSwinging)
            {
                StartSwing();
                isSwinging = true;
                playerMovement.readyToDoubleJump = false;
                playerMovement.hasJumpedInSwing = false;
            }
        }
        

        if (_input.swing && !isObjectGrappleActive)
        {
            StartGrappleObject();
            isObjectGrappleActive = true;
        }

        else if (!_input.swing) 
        {
            ReleaseGrappleObject();
            isObjectGrappleActive = false;
        }


        //stopping is always possible
        if (!_input.swing && isSwinging)
        {
            StopSwing();
            isSwinging = false;
        }
    }

    #region Swinging
    void StartSwing()
    {
        if (predictionHit.point == Vector3.zero)
        {
            return;
        }

        //customize these for the rope animation
        //spring.SetTarget(0f); // Desired amplitude of the rope's wave animation.
        //spring.SetValue(0f); // Initial value of the animation, starting fully 'waved'.
        spring.SetStrength(strength); // How 'strong' the spring pulls back to its target state.
        spring.SetDamper(damper); // Damping to slow down the animation over time.
        spring.SetVelocity(animationVelocity); // Initial velocity of the spring animation.
        //

        CancelActiveGrapple();
        playerMovement.ResetRestrictions();
        
        playerMovement.swinging = true;
        Vector3 swingStartVelocity = playerMovement.lastVelocity;
        rb.velocity = swingStartVelocity; 

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        //distance grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.15f;


        //customize these as we like
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lineRenderer.positionCount = 2;
        currentGrapplePosition = gunTip.position;
        posAtStartSwing = gameObject.transform.position;

        PlayRandomSound();
    }

    public void StopSwing()
    {
        // playerMovement.swinging = false;
        // lineRenderer.positionCount = 0;
        // Destroy(joint);
        
        // This code above is how it is originally supposed to look like
        // but there was a constant bug of the srping joints neverdisappearing and
        // staying attached to the player forever, keeping the player stuck within a certain range of it
        // The code below makes sure that every time StopSwing() happens, that it
        // destroys any and all instances of the joint.
        // Just giving this info out for any future cases of a similar error


        playerMovement.swinging = false;
        //playerMovement.readyToDoubleJump = true;
        lineRenderer.positionCount = 0;
        if (joint != null)
        {
            Destroy(joint);
            joint = null; // Set to null to ensure you don't try to access it again
        }
        else
        {
            // As a fallback, if for some reason the joint reference was lost,
            // destroy any SpringJoint attached to the player.
            SpringJoint existingJoint = player.GetComponent<SpringJoint>();
            if (existingJoint != null)
            {
                Destroy(existingJoint);
            }
        }
        
    }

    private void SwingingMovement()
    {
        bool isWPressed = _input.move.y > 0;
        bool isAPressed = _input.move.x < 0;
        bool isSPressed = _input.move.y < 0;
        bool isDPressed = _input.move.x > 0;

        

        //right
        if (isDPressed) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        //left
        if (isAPressed) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

        //forward
        if (isWPressed) rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);

        //shorten cable
        if (_input.shorten)
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * shortenCableSpeed * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.15f;
            //_input.shorten = false;
        }

        //extend cable
        // if (isSPressed)
        // {
        //     float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

        //     joint.maxDistance = extendedDistanceFromPoint * 0.8f;
        //     joint.minDistance = extendedDistanceFromPoint * 0.15f;
        // }
        //////////////////////////////////////////////////////////////////
        ///
        
        Vector3 swingToStart = (posAtStartSwing - swingPoint ).normalized;
        Vector3 swingToCurrent = (gameObject.transform.position - swingPoint).normalized;


        float swingAngleDelta = Vector3.Angle(swingToStart, swingToCurrent);
        
        if (swingAngleDelta >= swingAngleToJump && !playerMovement.hasJumpedInSwing)
        {
            playerMovement.readyToJumpAfterSwing = true;
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
            CrosshairIndicator.SetActive(true);
            
        }

        //realHitPoint not found
        else
        {
            predictionPoint.gameObject.SetActive(false);
            CrosshairIndicator.SetActive(false);
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
        isGrappleActive = true;

        spring.SetStrength(strength); // How 'strong' the spring pulls back to its target state.
        spring.SetDamper(damper); // Damping to slow down the animation over time.
        spring.SetVelocity(animationVelocity);


        if (predictionHit.point != Vector3.zero)
        {
            lineRenderer.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            swingPoint = predictionHit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }

        else
        {
            swingPoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        grapplingCdTimer = grapplingCd;

        PlayRandomSound();
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
        isGrappleActive = false;

        grapplingCdTimer = grapplingCd;

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

    #region GrapplingObject

    void StartGrappleObject()
    {
         RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxObjectGrappleDistance, whatIsGrappleObject))
        {
            // Clean up any existing swing joint before initiating object grapple
            //without this, we would be stuck to the initial swing point where we grappled the object
            //and the swing point will not move even though the object moved, essentially making us stuck to an invisible spot
            DestroySwingJoint();

            grappledObject = hit.transform;
            if (grappledObject.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                MeleeEnemyPathfinding melee = grappledObject.GetComponent<MeleeEnemyPathfinding>();
                RangedEnemyPathfinding ranged = grappledObject.GetComponent<RangedEnemyPathfinding>();

                if (ranged != null)
                {
                    ranged.GetGrappled();
                }
                else if (melee != null)
                {
                    melee.GetGrappled();
                }
            }
            objectJoint = player.gameObject.AddComponent<SpringJoint>();

            objectJoint.connectedBody = grappledObject.GetComponent<Rigidbody>();
            // Set the connected anchor in the object's local space
            objectJoint.connectedAnchor = grappledObject.InverseTransformPoint(hit.point);
            //objectJoint.connectedBody = grappledObject.GetComponent<Rigidbody>();
            objectJoint.autoConfigureConnectedAnchor = false;
            //objectJoint.connectedAnchor = hit.point - grappledObject.position;

            objectJoint.spring = objectGrappleSpring;
            objectJoint.damper = objectGrappleDamper;
            objectJoint.massScale = objectGrappleMassScale;
            objectJoint.connectedMassScale = objectConnectedMassScale;

            //adjust these as we want
            objectJoint.maxDistance = Vector3.Distance(player.position, grappledObject.position) * 0.8f;
            objectJoint.minDistance = 0.30f;

            // Initialize the line renderer for the grapple object
            if (lineRenderer != null && grappledObject != null)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, gunTip.position);
                lineRenderer.SetPosition(1, grappledObject.position);
            }
        }


        
    }

    void DragGrappleObject()
    {
        //calculate target position in front of the camera to drag the object towards
        if (grappledObject != null)
        {
            Vector3 targetPosition = cam.position + cam.forward * Vector3.Distance(cam.position, grappledObject.position);
            
            
            Vector3 forceDirection = (targetPosition - grappledObject.position).normalized;
            float forceMagnitude = 10f; // Adjust this value as needed
            grappledObject.GetComponent<Rigidbody>().AddForce(forceDirection * forceMagnitude);
        }
    }

    void ReleaseGrappleObject()
    {
        if (objectJoint != null)
        {
            //throw force based on mouse movement
            if (grappledObject != null)
            {
                Vector3 throwForce = Vector3.zero; //placeholder for throw force calculation
                
                //apply throw force to object's rigidbody
                grappledObject.GetComponent<Rigidbody>().AddForce(throwForce, ForceMode.VelocityChange);

                if (grappledObject.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    var script = grappledObject.GetComponent<MeleeEnemyPathfinding>();
                    if (script != null)
                    {            
                        grappledObject.GetComponent<MeleeEnemyPathfinding>().GetReleased();
                    }
                    
                    var script2 = grappledObject.GetComponent<RangedEnemyPathfinding>();
                    if (script2 != null)
                    {            
                        grappledObject.GetComponent<RangedEnemyPathfinding>().GetReleased();
                    }
                }
            }
            
            //cleanup
            Destroy(objectJoint);
            objectJoint = null;
            grappledObject = null;

            // Reset the line renderer when the grapple is released
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }
        }
    }

    private void DestroySwingJoint()
    {
        playerMovement.swinging = false;
        //playerMovement.readyToDoubleJump = true;
        lineRenderer.positionCount = 0;
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }

        // As a fallback, if for some reason the joint reference was lost,
        // destroy any SpringJoint attached to the player.
        SpringJoint existingJoint = player.GetComponent<SpringJoint>();
        if (existingJoint != null)
        {
            Destroy(existingJoint);
        }
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
        if (!joint && !isGrappleActive)
        {
            if (lineRenderer.positionCount > 0)
            {
                lineRenderer.positionCount = 0;
            }
            return;
        }

        
        if (isGrappleActive && !joint)
        {
            
            lineRenderer.positionCount = 2;
            
            lineRenderer.SetPosition(0, gunTip.position);
            lineRenderer.SetPosition(1, swingPoint);

            AnimateGrapple(swingPoint);
            return;
        }
        
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

    private void AnimateGrapple(Vector3 targetPosition)
    {
        if (lineRenderer.positionCount != quality + 1)
        {
            lineRenderer.positionCount = quality + 1;
        }

        spring.Update(Time.deltaTime);
        Vector3 swingDirection = (targetPosition - gunTip.position).normalized;
        Vector3 up = Quaternion.LookRotation(swingDirection) * Vector3.up;

        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);
            lineRenderer.SetPosition(i, Vector3.Lerp(gunTip.position, targetPosition, delta) + offset);
        }
    }

    private void AnimateGrappleObject(Transform targetTransform)
    {
        if (isObjectGrappleActive && grappledObject != null)
        {
            if (lineRenderer.positionCount != quality + 1)
            {
                lineRenderer.positionCount = quality + 1;
            }

            
            Vector3 targetPosition = targetTransform.position;
            Vector3 swingDirection = (targetPosition - gunTip.position).normalized;
            Vector3 up = Quaternion.LookRotation(swingDirection) * Vector3.up;

            spring.Update(Time.deltaTime);

            for (int i = 0; i < quality + 1; i++)
            {
                float delta = i / (float)quality;
                Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);
                lineRenderer.SetPosition(i, Vector3.Lerp(gunTip.position, targetPosition, delta) + offset);
            }
        }
    }

    void PlayRandomSound()
    {
        if (swingSounds.Length > 0)
        {
            int index = Random.Range(0, swingSounds.Length);
            audioSource.clip = swingSounds[index];
            audioSource.Play();
        }
    }
}