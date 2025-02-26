using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHiding : MonoBehaviour
{
    private Collider playerCollider; // Reference the the player's Collider

    private PlayerMovement playerMovement; // Reference to PlayerMovement script to access the player's crouch state

    private bool canHide = false; // Determines if the player is actively able to hide (Default = False)
    public bool isHidden = false; // Detemines if the player is currently hidden (Default = False)
    
    public GameObject isHiddenPanel; // Reference to the is Hidden panel

    // Start is called before the first frame update
    void Start()
    {
        // Get the player's collider component
        playerCollider = GetComponentInChildren<Collider>();
        // Get the playerMovement Script
        playerMovement = GetComponent<PlayerMovement>();

        // Ensure the "isHiddenPanel" is initially hidden when the game starts
        if (isHiddenPanel != null)
        {
            isHiddenPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the player can hide...
        if(canHide)
        {
            // And if the player is currently in a crouching state...
            if(playerMovement.state == PlayerMovement.MovementState.crouching)
            {
                // Make the player hidden
                isHidden = true;
            } 
            // Else remove hidden state
            else
            {
                isHidden = false;
            }
        }
        // If the player cannot hide, ensure the isHidden flag is set to false
        else
        {
            isHidden = false;
        }
        // Enable/Disable the "isHiddenPanel" based on whether the player is hidden or not
        if (isHiddenPanel != null)
        {
            isHiddenPanel.SetActive(isHidden); // Enable the panel when hidden, disable when not
        }
    }

    // This method is called when the collider of the child object hits something
    public void OnTriggerEnter(Collider other)
    {
        // Check if the object collided with has the tag "Wall"
        if (other.CompareTag("Hiding Zone"))
        {
            Debug.Log("Player has entered a Hiding Zone");
            // Set canHide to true
            canHide = true; 
        }
    }
    // This method is called when the collider of the child object hits something
    public void OnTriggerExit(Collider other)
    {
        // Check if the object collided with has the tag "Wall"
        if (other.CompareTag("Hiding Zone"))
        {
            Debug.Log("Player has exited a Hiding Zone");
            // Set canHide to true
            canHide = false; 
        }
    }
}
