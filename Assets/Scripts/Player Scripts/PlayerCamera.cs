using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using StarterAssets;


public class PlayerCamera : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public float controllerSensitivity;
    public float multiplier;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    [Header("Fov")]
    public bool useFluentFov;
    public PlayerController playerMovement;
    public Rigidbody rb;
    public Camera cam;
    public float minMovementSpeed;
    public float maxMovementSpeed;
    public float minFov;
    public float maxFov;

    // For new input system
    private StarterAssetsInputs _input;
    private PlayerInput _playerInput;
    private const float _threshold = 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
            #if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
            #else
			    return false;
            #endif
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _input = GetComponent<StarterAssetsInputs>();
        #if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
        #else
		    Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
        #endif
            }

    private void Update()
    {
        if (_input.look.sqrMagnitude >= _threshold && !playerMovement.paused && !playerMovement.dead)
        {

            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            float xSens = IsCurrentDeviceMouse ? sensX : controllerSensitivity;
            float ySens = IsCurrentDeviceMouse ? sensY : controllerSensitivity;

            // get mouse input
            float mouseX = _input.look.x * xSens * deltaTimeMultiplier;
            float mouseY = _input.look.y * ySens * deltaTimeMultiplier;

            yRotation += mouseX * multiplier;

            xRotation += mouseY * multiplier;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // rotate cam and orientation
            camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);

            if (useFluentFov) HandleFov();
        }

        
    }

    private void HandleFov()
    {
        float moveSpeedDif = maxMovementSpeed - minMovementSpeed;
        float fovDif = maxFov - minFov;

        float rbFlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        float currMoveSpeedOvershoot = rbFlatVel - minMovementSpeed;
        float currMoveSpeedProgress = currMoveSpeedOvershoot / moveSpeedDif;

        float fov = (currMoveSpeedProgress * fovDif) + minFov;

        float currFov = cam.fieldOfView;

        float lerpedFov = Mathf.Lerp(fov, currFov, Time.deltaTime * 200);

        cam.fieldOfView = lerpedFov;
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
}