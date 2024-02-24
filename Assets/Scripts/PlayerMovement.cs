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

        movement,
        wallrunning,
        air,
        crouch

    }

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
        if(grounded)
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

    private void StateHandler()
    {

          if(grounded)
        {
            state = MovementState.movement;
            moveSpeed = movementSpeed;
        }

        else if(wallrunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
        }
        //crouching mode
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouch;
            moveSpeed = crouchSpeed;
        }

        else
        {
            state = MovementState.air;
        }

    }

    private void MovePlayer()
    {
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


}
