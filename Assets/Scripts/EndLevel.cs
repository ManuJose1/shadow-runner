using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{

    public GameObject endScreen; 
  void OnCollisionEnter(Collision col)
  {
    if(col.gameObject.name == "Player")
    {
    Debug.Log("Level Ended");
    endScreen.SetActive(true);
    Time.timeScale = 0f;
    Cursor.lockState = CursorLockMode.Confined;
    Cursor.visible = true;
    }
  }   
}    
