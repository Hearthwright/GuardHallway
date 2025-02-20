using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float sensX; // Determines the sensitivity of the player's camera over the X-axis
    public float sensY; // Determines the sensitivity of the player's camera over the Y-axis

    public Transform orientation; // Handles the current orientation of the player's camera

    float xRotation; // Determines the player camera's current X rotation
    float yRotation; // Determines the player camera's current Y rotation


    // Start is called before the first frame update
    void Start()
    {
        // Lock player's cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Mouse Input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // Update Rotation
        yRotation += mouseX;
        xRotation -= mouseY;

        // Prevent the player from looking too far up or down by clamping the X Rotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate and Orient the player's camera
        // Rotating Camera along both axes
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        //Rotate player along Y-axis
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
