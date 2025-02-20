using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour
{
    public Transform target; // Target that the guard will attempt to move to
    private NavMeshAgent agent; // Nave Mesh Agent to determine how guard interacts with the nav mesh

    [Header("Detection Settings")]
    public float detectionRange = 10f; // Range at which the guard will begin to pursue the target

    [Header("Patrol Settings")]
    public float minRotationAmount = 15f; // The minimum value of the amount the guard can rotate after hitting a wall
    public float maxRotationAmount = 90f; // The maximum value of the amount the guard can rotate after hitting a wall
    public float rotationDelay = 2f; // The amount of time between rotations, prvents the guard from constantly adjusting roation while against a wall
    private Vector3 forwardDirection; // The direction the guard is facing and moving in
    private Vector3 lastDirection; // The last direction the guard was facing before losing sight of the target

    private bool isCollidingWithWall = false; // Flag to detect wall collisions
    private bool canRotate = true; // Determines if the guard can rotate now or not
    private bool isRotating = false; // Determines if the guard is currently rotating after hitting a wall

    private float rotationSpeed = 180f; // Speed of the rotation (degrees per second)


    // Start is called before the first frame update
    void Start()
    {
        // Get the NavMesh Agent assigned to the guard
        agent = GetComponent<NavMeshAgent>();

        // Set initial Patrol direction
        forwardDirection = transform.forward;
        lastDirection = forwardDirection;
    }

    // Update is called once per frame
    void Update()
    {
        // If the guard is rotating, do nothing
        if(isRotating)
        {
            return;
        }
        // Check distance to target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // If target is within the guard's detection range...
        if(distanceToTarget <= detectionRange)
        {
            // Pursue the target
            PursueTarget();
        }
        else
        {
            Patrol();
        }

        // If the guard collides with a wall
        if (isCollidingWithWall && !isRotating)
        {
            StartCoroutine(RotateRandomly());
            isCollidingWithWall = false; // Reset collision flag
        }
    }

    private void Patrol()
    {
        // Move the guard straight in the direction they are facing
        agent.SetDestination(transform.position + lastDirection);
    }

    // Rotates the guard a random amount within a set range
    private IEnumerator RotateRandomly()
    {
        // Disable rotation until the delay is over
        canRotate = false; 
        isRotating = true;

        // Randomly choose whether to rotate left (-1) or right (1)
        int rotationDirection = Random.Range(0, 2) == 0 ? -1 : 1;

        // Randomly rotate the guard by a set amount
        float randomRotation = Random.Range(minRotationAmount, maxRotationAmount) * rotationDirection;

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + randomRotation, 0);

        // Smoothly rotate towards the target rotation
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f) // Continue rotating until close to target rotation
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Final adjustment to ensure exact target rotation
        transform.rotation = targetRotation;

        // Update the direction the guard is moving towards based on the new rotation
        lastDirection = transform.forward;

        // Wait for the specified delay before allowing another rotation
        yield return new WaitForSeconds(rotationDelay);

        // Enable rotation after the delay
        canRotate = true;
        isRotating = false;
    }

    // Allows the guard to pursue a target directly
    private void PursueTarget()
    {
        // Set the agent's destination to the target's position
        agent.destination = target.position;

        // Update the last known direction to be the direction the guard is currently facing
        lastDirection = transform.forward;
    }

    // This method is called when the collider of the child object hits something
    public void OnTriggerEnter(Collider other)
    {
        // Check if the object collided with has the tag "Wall"
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Roomba Collided with Wall in Guard Controller");
            isCollidingWithWall = true; // Set flag to rotate guard
        }
    }
}
