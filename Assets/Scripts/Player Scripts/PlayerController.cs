using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float swingSpeed;
    public float airSpeed;
    public float maxAirSpeed = 27f;
    public float maxGroundSpeed = 7f;

    public float groundDrag;

    public Vector3 lastVelocity;

    [Header("Jumping")]
    public float jumpForce;
    public float doubleJumpForce;
    public float jumpAfterSwingForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float canDoubleJumpTimeout = 0.30f;
    
    bool readyToJump;
    public bool readyToDoubleJump;

    public bool readyToJumpAfterSwing;

    float canDoubleJumpDelta;

    bool hasLanded;
    public bool hasJumpedInSwing = false;

    // [Header("Crouching")]
    // public float crouchSpeed;
    // public float crouchYScale;
    // private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    // [Header("Slope Handling")]
    // public float maxSlopeAngle;
    // private RaycastHit slopeHit;
    // private bool exitingSlope;

    [Header("Camera Effects")]
    public PlayerCamera cam;
    public float grappleFov = 95f;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        grappling,
        swinging,
        walking,
        sprinting,
        crouching,
        air
    }

    public bool freeze;

    private bool enableMovementOnNextTouch;

    public bool activeGrapple;
    public bool swinging;

    private StarterAssetsInputs _input;


    // Escape stuff, feel free to change

    public bool paused;
    public bool dead = false;
    public GameObject pauseScreen;
    public GameObject winCanvas;

    [Header("Player Audio")]
    public AudioClip[] foostepSounds;
    public AudioSource footstepSource;
    public float footstepTimeout = 3.0f;
    private float footstepMagnitude = 0.0f;
    private Vector3 prevPos;
    public AudioMixer audioMixer;

    private void Start()
    {
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        _input = GetComponent<StarterAssetsInputs>();
        
        
        rb.freezeRotation = true;

        readyToJump = true;
        readyToDoubleJump = false;
        hasLanded = false;
        canDoubleJumpDelta = canDoubleJumpTimeout;
        prevPos = transform.position;

        
        //startYScale = transform.localScale.y;

        lastVelocity = Vector3.zero;
    }

    private void Update()
    {
        // ground check

        bool prevGrounded = grounded;

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (prevGrounded != grounded && grounded)
            hasLanded = true;

        if (!dead)
        {
            MyInput();
        }
        SpeedControl();
        StateHandler();

        if (readyToDoubleJump && canDoubleJumpDelta >= 0.0f)
            canDoubleJumpDelta -= Time.deltaTime;

        // handle drag
        if (grounded && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        CheckFootstepSound();

        lastVelocity = rb.velocity;

    }

    private void FixedUpdate()
    {
        MovePlayer();
        SpeedControl();
    }

    private void MyInput()
    {
        horizontalInput = _input.move.x;
        verticalInput = _input.move.y;

        

        // when to jump
        if (_input.jump && readyToJump && grounded)
        {
            readyToJump = false;
            canDoubleJumpDelta = canDoubleJumpTimeout;
            Jump();
            readyToDoubleJump = true;

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (_input.jump && readyToDoubleJump && canDoubleJumpDelta <= 0.0f)
        {
            canDoubleJumpDelta = canDoubleJumpTimeout;
            Jump();
            readyToDoubleJump = false;

            //Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (_input.jump && readyToJumpAfterSwing)
        {
            Jump();
            readyToJumpAfterSwing = false;
            hasJumpedInSwing = true;
        }

        if (_input.pause) // NEEDS TO BE CHANGED TO STANDARD KEY, DUNNO HOW TO DO - Ágúst
        {
            PauseGame();
            _input.pause = false;
        }

        _input.jump = false;

        // // start crouch
        // if (Input.GetKeyDown(crouchKey))
        // {
        //     transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        //     rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        // }

        // // stop crouch
        // if (Input.GetKeyUp(crouchKey))
        // {
        //     transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        // }
    }

    public void PauseGame() // Needs to be it's own function so that the pause screen can deactivate pause as well
    {
        
        if (paused)
        {
            pauseScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;

            paused = !paused;
            Time.timeScale = 1;
            Cursor.visible = false;
            audioMixer.SetFloat("CutoffParam", 0.5f);
        }
        else if (!paused)
        {
            pauseScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            paused = !paused;
            Time.timeScale = 0;
            Cursor.visible = true;
            audioMixer.SetFloat("CutoffParam", 0.5f);
        }
    }

    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }

        // Mode - Grappling
        else if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = sprintSpeed;
        }

        // Mode - Swinging
        else if (swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed;
        }

        // // Mode - Crouching
        // else if (Input.GetKey(crouchKey))
        // {
        //     state = MovementState.crouching;
        //     moveSpeed = crouchSpeed;
        // }

        // Mode - Sprinting
        else if (grounded && _input.sprint)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            
            canDoubleJumpDelta = canDoubleJumpTimeout;
            
        }

        if (hasLanded)
        {
            readyToJump = true;
            readyToDoubleJump = false;
            hasLanded = false;
            readyToJumpAfterSwing = false; 
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
            moveSpeed = airSpeed;
        }

        if (!grounded)
        {
            _input.jump = false;
        }
    }

    private void MovePlayer()
    {


        //Stops player move when grappling
        if (activeGrapple) return;
        if (swinging) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;



        if (swinging || grounded)
        {
            float dynamicSpeed = CalculateDynamicSpeed();
            Vector3 forceToApply = moveDirection.normalized * dynamicSpeed * 10f * (grounded ? 1f : airMultiplier);
            rb.AddForce(forceToApply, ForceMode.Force);
        }
        //air movement (not swinging)
        else if (!grounded && !swinging)
        {
            //allow directional change without affecting speed.
            AdjustAirDirection(moveDirection);
        }
    }

    private void AdjustAirDirection(Vector3 inputDirection)
    {
        
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            inputDirection = inputDirection.normalized;
            
            float currentSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
            
            Vector3 newVelocity = inputDirection * currentSpeed;
            
            newVelocity.y = rb.velocity.y;
            
            rb.velocity = newVelocity;
        }
    }

    private float CalculateDynamicSpeed()
    {
        //dynamic speed calculation. Replace as needed
        float speed = moveSpeed;
        if (swinging)
        {
             //use either the swingSpeed or the last known velocity, whichever is greater
            speed = Mathf.Max(swingSpeed, lastVelocity.magnitude);
        }
        else if (!grounded)
        {
            //similar here
            speed = Mathf.Max(airSpeed, lastVelocity.magnitude);
        }
        return speed;
    }

    private void SpeedControl()
    {

         //preserve momentum when swinging or in air, applying max speed constraints otherwise
        if (!swinging && state == MovementState.air)
        {
            //enforce maxAirSpeed only if gaining speed through air control mechanics
            if (rb.velocity.magnitude > maxAirSpeed)
            {
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxAirSpeed);
            }
        }
        else if (grounded && !activeGrapple)
        {
            //enforce maxGroundSpeed on the ground
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > maxGroundSpeed)
            {
                rb.velocity = new Vector3(flatVel.normalized.x * maxGroundSpeed, rb.velocity.y, flatVel.normalized.z * maxGroundSpeed);
            }
        }
    }

    private void Jump()
    {

        // reset y velocity
        // rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // if (readyToDoubleJump)
        //     rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
        // else if (readyToJumpAfterSwing)
        //     rb.AddForce(transform.up * jumpAfterSwingForce, ForceMode.Impulse);
        // else
        // rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        //new jump implementation
        Vector3 jumpDirection = Vector3.up * jumpForce;
        if (readyToDoubleJump || readyToJumpAfterSwing)
        {
            jumpDirection = Vector3.up * Mathf.Max(jumpForce, lastVelocity.y + jumpForce); 
        }
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); 
        rb.AddForce(jumpDirection, ForceMode.Impulse);

    }
    private void ResetJump()
    {
        readyToJump = true;

        //exitingSlope = false;
    }

    public void JumpToPosition(Vector3 targetposition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetposition, trajectoryHeight);

        Invoke(nameof(SetVelocity), 0.1f);

        //if grappled too long, in this case 3 seconds, then enable movement again
        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;

        // cam.DoFov(grappleFov);
        
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
        // cam.DoFov(85f);
    }

    private void CheckFootstepSound()
    {
        //footstepTimeoutCount += Time.deltaTime;

        if (footstepMagnitude >= footstepTimeout && grounded)
        {
            int randval = Random.Range(0, foostepSounds.Length);
            footstepSource.PlayOneShot(foostepSounds[randval]);
            footstepMagnitude = 0.0f;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            //GetComponent<Swinging>().StopGrapple();
            
        }
    }

    public void WinTrigger()
    {
        winCanvas.SetActive(true);
    }

}
