using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement settings")]
    private float moveSpeed;
    public float movementSpeed;
    public float groundDrag;
    public float wallrunSpeed;
    public float swingSpeed;
    public float shiftSpeed;
    public float shiftSpeedChangeFactor;
    public float maxYSpeed;



    [Header("jump settings")]
    public float jumpHeight;
    public float jumpcooldown;
    public float airMultiplier;
    bool readyTojump;

    [Header("crouch settings")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;



    [Header("Movement settings")]
    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground detect")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

     [Header("Slope Handle")]
     public float maxSlopeAngle;
     private RaycastHit SlopeHit;
     private bool exitSlope;
 


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState
    {
        freeze,
        movement,
        swinging,
        shifting,
        wallrunning,
        air,
        crouch

    }


    public bool freeze;
    public bool activeGrapple;
     public bool shifting;
    public bool swinging;
    public bool wallrunning;

   private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyTojump = true;   
        //state before crouch
        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        //checking if touching ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        //adds drag if touching groundd
        if(grounded && !activeGrapple )
            rb.drag = groundDrag;
        else 
            rb.drag = 0;   
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }    

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical"); 

        //checks if jump cooldown is completed and is on the ground
        if(Input.GetKey(jumpkey) && readyTojump && grounded) 
        {
            readyTojump = false;

            Jump();

            Invoke(nameof(ResetJump),jumpcooldown);
        } 


        //crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp (crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x,startYScale, transform.localScale.z);
        }
    }

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    private void StateHandler()
    {

        if (freeze)
        {
            state = MovementState.freeze;
            desiredMoveSpeed = 0;
            rb.velocity = Vector3.zero;
        }

        else if(grounded)
        {
            state = MovementState.movement;
            desiredMoveSpeed = movementSpeed;
        }

        else if (shifting)
        {
            state = MovementState.shifting;
            desiredMoveSpeed = shiftSpeed;
            speedChangeFactor = shiftSpeedChangeFactor;
        }

         else if(swinging)
        {
            state = MovementState.swinging;
            desiredMoveSpeed = swingSpeed;
        }

        else if(wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }
        //crouching mode
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouch;
            desiredMoveSpeed = crouchSpeed;
        }

        else
        {
            state = MovementState.air;
            if(desiredMoveSpeed < movementSpeed)
               desiredMoveSpeed = movementSpeed;
            else
                desiredMoveSpeed = movementSpeed;
            
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.shifting) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if(keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());

            }
            else
            {
                StopAllCoroutines();   
                moveSpeed =desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;

    }

    private float speedChangeFactor;

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MovePlayer()
    {
        if(state == MovementState.shifting) return;

        if (activeGrapple) return;
        if (swinging) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitSlope)
        {
            rb.AddForce(GetSlopeMovementDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }


        if(grounded)
            //if on ground
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force );

        // in the air
        else if(!grounded)
         rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        if(!wallrunning) rb.useGravity = !OnSlope();


    }

    private void SpeedControl()
    {
        if (activeGrapple) return;

        if (OnSlope() && !exitSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

         if(flatVel.magnitude > moveSpeed)
             {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
             }
        }

        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    

    }

    //jump method
    private void Jump()
    {
        exitSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f ,rb.velocity.z);
        rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyTojump = true;
        exitSlope = false;
    }


    private bool enableMovementOnNextTouch;


    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = calculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
      Invoke(nameof(SetVelocity), 0.1f);

      Invoke(nameof(ResetRestrictions),3f);
    }

    private Vector3 velocityToSet;

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }       
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out SlopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, SlopeHit.normal);
            return angle < maxSlopeAngle && angle !=0;
        }
        return false;
    }

    private Vector3 GetSlopeMovementDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection,SlopeHit.normal).normalized;
    }

    public Vector3 calculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ/ (Mathf.Sqrt(-2 * trajectoryHeight/gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }


}
