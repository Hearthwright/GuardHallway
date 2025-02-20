using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSensorCollision : MonoBehaviour
{
    private GuardController guardController;

    // Start is called before the first frame update
    void Start()
    {
        // Get the parent GuardController
        guardController = GetComponentInParent<GuardController>();
        if(guardController != null)
        {
            Debug.Log("Guard Controller Found");
        } 
    }

    // Called when the collider on the Roomba's front sensor is activated
    private void OnTriggerEnter(Collider other)
    {
        if (guardController != null)
        {
            // Call the parent method to handle the collision
            guardController.OnTriggerEnter(other);

            Debug.Log("Roomba Collided with Wall");
        }
    }
}
