using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shifting : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("shifting")]
    public float shiftForce;
    public float shiftUpwardForce;
    public float maxShiftYSpeed;
    public float shiftDuration;

    [Header("CameraEffects")]
    public PlayerCamera cam;
    public float dashFov;


    [Header("settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float shiftCd;
    private float shiftCdTimer;

    [Header("Input")]

    public KeyCode shiftKey = KeyCode.LeftShift;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(shiftKey))
        Shift();

        if(shiftCdTimer > 0)
            shiftCdTimer -= Time.deltaTime;

    }

    private void Shift()
    {

        if(shiftCdTimer > 0) return;
        else shiftCdTimer = shiftCd;

        pm.shifting = true;
        pm.maxYSpeed = maxShiftYSpeed;

        cam.DoFov(dashFov);

        Transform forwardT;

        if(useCameraForward)
            forwardT = playerCam;
        else
            forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * shiftForce + orientation.up * shiftUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedShiftForce), 0.025f);

        Invoke(nameof(ResetShift), shiftDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedShiftForce()
    {   
        if(resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetShift()
    {
              pm.shifting = false;
              pm.maxYSpeed = 0;

              cam.DoFov(70f);

              if(disableGravity)
              rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if(allowAllDirections)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;
        
        if(verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;


    }


}
