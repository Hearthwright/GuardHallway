using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardSensor : MonoBehaviour
{
    private SceneController sceneController; // Reference to Scene Controller, Used to end the game if the player is hit
    private GuardController guardController; // Reference to Guard Controller to handle Bouncing off walls

    // Start is called before the first frame update
    void Start()
    {
        // Locate Scene Controller Object
        sceneController = FindObjectOfType<SceneController>();
        // Locate guardController Script
        guardController = GetComponent<GuardController>();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision Detected: " + other.gameObject.name);
        // If guard collides with the player
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Roomba Collided with Player");
            // End Game
            sceneController.GameOver();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "BounceTrigger")
        {
            Debug.Log("Roomba Collided with Wall");
            // Bounce the Guard off the wall
            guardController.Bounce();
        }
    }
}
