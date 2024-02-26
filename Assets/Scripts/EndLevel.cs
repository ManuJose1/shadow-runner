using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("LevelEnding"))
        {
            Debug.Log("Level Complete");
            EventManager.OnTimerStop();
        }
    }
}
