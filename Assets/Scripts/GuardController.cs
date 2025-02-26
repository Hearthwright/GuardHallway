using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 10f; // Range at which the guard will begin to pursue the target
    public float crouchModifier = 0.5f; // The modifier added to the guard's detection range when the player is crouched (Default = 0.5f)
    public Transform target; // Target that the guard will attempt to move to. This should be the Player's position

    [Header("Boucne Settings")]
    public float minRotationAmount = 15f; // The minimum value of the amount the guard can rotate after hitting a wall
    public float maxRotationAmount = 90f; // The maximum value of the amount the guard can rotate after hitting a wall
    public float bounceInterval = 2f; // The amount of time between bounces, prvents the guard from constantly adjusting roation while against a wall
    private bool isBouncing = false; // Determines if the guard is currently bouncing to prevent the method from being called repeatedly

    private Vector3 forwardDirection; // The direction the guard is facing and moving in
    private Vector3 lastDirection; // The last direction the guard was facing before losing sight of the target

    private float rotationSpeed = 180f; // Speed of the rotation (degrees per second)

    private PlayerMovement playerMovement; // Reference to PlayerMovement script to detect if the player is crouching
    private PlayerHiding playerHiding; // Reference to the PlayerHiding script to check if the player is hidden
    private NavMeshAgent agent; // Nav Mesh Agent to determine how guard interacts with the nav mesh


    // Start is called before the first frame update
    void Start()
    {
        // Get the NavMesh Agent assigned to the guard
        agent = GetComponent<NavMeshAgent>();
        // Find the PlayerHiding script on the player
        playerHiding = target.GetComponent<PlayerHiding>();
        playerMovement = target.GetComponent<PlayerMovement>();

        // Set initial Patrol direction
        forwardDirection = transform.forward;
        // Set the last direction the guard was patroling
        lastDirection = forwardDirection;
    }

    // Update is called once per frame
    void Update()
    {
        // If the guard is bouncing, do nothing
        if(isBouncing)
        {
            return;
        }

        // Check distance to target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        // If the player is not in the spawn or end area
        if(!playerMovement.isInSpawnArea && !playerMovement.isInEndArea)
        {
            // If the player is crouching, halve the guard's detection range
            if(playerMovement.state == PlayerMovement.MovementState.crouching)
            {
                // If target is within the guard's detection range and player is not hidden
                if(distanceToTarget <= detectionRange * 0.5f && !playerHiding.isHidden)
                {
                    // Pursue the target
                    PursueTarget();
                }
                // Else patrol randomly
                else
                {
                    Patrol();
                }
            }
            // Otherwise calculate range normally
            else
            {
                // If target is within the guard's detection range and player is not hidden
                if(distanceToTarget <= detectionRange && !playerHiding.isHidden)
                {
                    // Pursue the target
                    PursueTarget();
                }
                // Else patrol randomly
                else
                {
                    Patrol();
                }
            }
        }
        // Else patrol randomly
        else
        {
            Patrol();
        }
    }

    // Allows the guard to pursue a target directly
    private void PursueTarget()
    {
        // Set the agent's destination to the target's position
        agent.destination = target.position;

        // Update the last known direction to be the direction the guard is currently facing
        lastDirection = transform.forward;
    }

    private void Patrol()
    {
        // Move the guard straight in the direction they are facing
        agent.SetDestination(transform.position + lastDirection);
    }

    public void Bounce()
    {
        Debug.Log("Starting Bounce");
        // If the guard collides with a wall
        if (!isBouncing)
        {
            // Set isBouncing to true to prevent repeated bounces
            isBouncing = true;
            // Prevent Guard from moving during bounce
            agent.isStopped = true;
            // Start Rotating randomly
            StartCoroutine(RotateRandomly());
        }
    }

    // Rotates the guard a random amount within a set range
    private IEnumerator RotateRandomly()
    {
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
        yield return new WaitForSeconds(bounceInterval);

        // Enable rotation after the delay
        agent.isStopped = false;
        isBouncing = false;
    }
}
