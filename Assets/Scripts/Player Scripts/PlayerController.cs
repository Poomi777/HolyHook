using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //https://www.youtube.com/watch?v=TYzZsBl3OI0&t=183s&ab_channel=Dave%2FGameDevelopment 
    //start at around 5:00 to continue the implementation

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float swingSpeed;
    public float airSpeed;

    public float groundDrag;

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

    }

    private void FixedUpdate()
    {
        MovePlayer();
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
        }
        else if (!paused)
        {
            pauseScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            paused = !paused;
            Time.timeScale = 0;
            Cursor.visible = true;
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

        // // on slope
        // if (OnSlope() && !exitingSlope)
        // {
        //     rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

        //     if (rb.velocity.y > 0)
        //         rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        // }

        
        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }

        // in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        Vector3 newPos = transform.position;


        float magn = (newPos - prevPos).magnitude;
        footstepMagnitude += magn;
        prevPos = transform.position;
        // turn gravity off while on slope
        //rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        //stops player from speed while grappling
        //if (activeGrapple) return;

        // // limiting speed on slope
        // if (OnSlope() && !exitingSlope)
        // {
        //     if (rb.velocity.magnitude > moveSpeed)
        //         rb.velocity = rb.velocity.normalized * moveSpeed;
        // }

        // limiting speed on ground or in air
        // else
        // {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        // }
    }

    private void Jump()
    {
        //exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (readyToDoubleJump)
            rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
        else if (readyToJumpAfterSwing)
            rb.AddForce(transform.up * jumpAfterSwingForce, ForceMode.Impulse);
        else
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }
    private void ResetJump()
    {
        readyToJump = true;

        //exitingSlope = false;
    }

    // private bool OnSlope()
    // {
    //     if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
    //     {
    //         float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
    //         return angle < maxSlopeAngle && angle != 0;
    //     }

    //     return false;
    // }

    // private Vector3 GetSlopeMoveDirection()
    // {
    //     return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    // }

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

        cam.DoFov(grappleFov);
        
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
        cam.DoFov(85f);
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

            GetComponent<Swinging>().StopGrapple();
            
        }
    }

    public void WinTrigger()
    {
        winCanvas.SetActive(true);
    }

}
