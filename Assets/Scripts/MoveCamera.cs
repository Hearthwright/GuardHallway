using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition; // Determines the current position the camera should be at

    // Update is called once per frame
    void Update()
    {
        // Move Camera to the intended position as determined by Camera Position
        transform.position = cameraPosition.position;
    }
}
