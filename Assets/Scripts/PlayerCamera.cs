using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PlayerCamera : MonoBehaviour
{
  
    public float PlayerSensX;
    public float PlayerSensY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }   


    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * PlayerSensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * PlayerSensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


    }

    public void DoFov(float endValue)
    {
     GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

     public void DoTilt(float zTilt)

    {
        transform.DOLocalRotate(new Vector3(0,0,zTilt), 0.25f);
    }





}
