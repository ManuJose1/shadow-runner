using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour
{

[Header("References")]
private PlayerMovement pm;
public LineRenderer lr;
public Transform cam;
public Transform gunTip;
public Transform player;
public LayerMask whatIsGrappleable;

[Header("References")]
public Transform orientation;
public Rigidbody rb;
public float horizontalThrustForce;
public float forwardThrustForce;
public float extendCableSpeed;


[Header("Swinging")]
public float maxSwingDistance;
private Vector3 swingPoint;
private SpringJoint joint; 

[Header("Input")]
 public KeyCode swingKey = KeyCode.Mouse0;


  private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }
   void Update()
    {
        if(Input.GetKeyDown(swingKey)) StartSwing();
        if(Input.GetKeyUp(swingKey)) StopSwing();

        if (joint !=null) airmovement();
        
    }

    private void LateUpdate()
    {
        DrawRope();
    }

   void StartSwing()
    {
        RaycastHit hit;
        pm.swinging =true;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

             joint.spring = 4.5f;
             joint.damper = 7f;
             joint.massScale = 4.5f;

             lr.positionCount = 2;
             currentGrapplePosition = gunTip.position;
        }

    }

    void StopSwing()
    {
        pm.swinging =false;
        lr.positionCount = 0;
        Destroy(joint);

    }

    private void airmovement()
    {
           
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
       
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

  
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);

       
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
       
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {

        if(!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition,swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);

    }

}
