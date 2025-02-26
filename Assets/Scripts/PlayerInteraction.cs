using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E; // Determines the button for interact
    public float interactionDistance = 5f; // How far the player can interact

    public Camera playerCamera; // The camera to check for raycasting
    private RaycastHit hit;

    private SceneController sceneController;

    void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
    }

    void Update()
    {
        // Cast a ray from the camera forward
        Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Check if the item hit by raycast is a power button
            GuardPowerButton powerButton = hit.collider.GetComponent<GuardPowerButton>();
            // If so...
            if (powerButton != null)
            {
                // If player hits the interact key
                if (Input.GetKeyDown(interactKey))
                {
                    // Freeze Guard
                     powerButton.PressButton();
                }
            }
            WinObject winObject = hit.collider.GetComponent<WinObject>();
            if(winObject != null)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    // Freeze Guard
                    sceneController.Win();
                }
            }
        }
    }
}
