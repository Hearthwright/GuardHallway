using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    private float moveSpeed; // Determines how fast the player is able to move
    public float walkSpeed; // Determines the speed the player moves at while walking
    public float sprintSpeed; // Determines the speed the player moves at while sprinting
    public float groundDrag; // Determines the amount of drag experienced whilst grounded

    [Header("Jump Settings")]
    public float jumpForce; // Determines the amount of force applied when jumping
    public float jumpCooldown; // Determines the amount of time between jumps
    public float airMultiplier; // Determines how speed is effected while airborne
    bool canJump; // Determines if the player is currently allowed to jump

    [Header("Crouch Settings")]
    public float crouchSpeed; // Determines the player's move speed whilst crouching
    public float crouchYScale; // Determines the player's Y Scale whilst crouching
    private float startYScale; // Tracks the player's initial/default Y Scale

    [Header("Keybind Settings")]
    public KeyCode jumpKey = KeyCode.Space; // Determines which key handles jumping (Default = Space)
    public KeyCode sprintKey = KeyCode.LeftShift; // Determines which key handles Sprinting (Default = Left Shift)
    public KeyCode crouchKey = KeyCode.LeftControl; // Determines which key handles Crouching (Default = Left Ctrl)

    [Header("Ground Check")]
    public float playerHeight; // Stores the player's height
    public LayerMask whatIsGround; // Determines what surfaces are the ground
    bool grounded; // Determines if the player is currently in a grounded state

    [Header("Slope Handling")]
    public float maxSlopeAngle; // The maximum angle a surface can be at before a player ceases being able to move up it
    private RaycastHit slopeHit; // Checks the slope of the current surface
    private bool exitingSlope; // Determines if the player is actively exiting a slope

    [Header("Misc.")]
    public Transform orientation; // Handles the player's current orientation
    public bool isInSpawnArea; // Tracks if the player is in the starting area
    public bool isInEndArea; // Tracks if the player is in the ending area

    float horizontalInput; // Determines the player's current Horizontal Input
    float verticalInput; // Determines the player's current Vertical Input

    Vector3 moveDirection; // Determines the direction the player is moving in

    Rigidbody rb; // Reference to the player's rigidbody

    public MovementState state; // Stores the player's current Movement State

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    // Start is called before the first frame update
    void Start()
    {
        // Assign the rigidbody and freeze its rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        canJump = true;

        // Get player's current Y-Scale
        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Checking if player is currently grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        // Getting Player Input
        MyInput();
        // Limit Player's Max Speed
        SpeedControl();
        // Get Player's current State
        StateHandler();
        // Apply Drag
        if(grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Gets player's input
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Determines when the player jumps
        if(Input.GetKey(jumpKey) && canJump && grounded)
        {
            // Disable the player's ability to jump
            canJump = false;

            Jump();

            // Re-enable the ability to jump after jumpCooldown
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        // Determines when player starts crouching
        if(Input.GetKeyDown(crouchKey))
        {
            // Changes player Scale
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            // Adds force to player to ground them
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        // Determines when the player stops crouching
        if(Input.GetKeyUp(crouchKey))
        {
            // Changes player Scale back to default
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Set Mode to crouching
        if(Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        // Set Mode to sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // Set Mode to walking
        else if(grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Set Mode to air
        else
        {
            state = MovementState.air;
        }
    }

    // Moves the player along the X and Z axis
    private void MovePlayer()
    {
        // Calculate Movement Direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Normalize the direction to avoid speed boost on diagonals
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
        
        // If the player is moving on a Slope
        if(OnSlope() && !exitingSlope)
        {
            // Add force in the direction of the slope
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            // Add downward force to prevent "bumpiness"
            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // If the player is grounded
        if(grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        // If the player is airborne
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // Disable Gravity whilst on Slope
        rb.useGravity = !OnSlope();
    }

    // Limits player Speed
    private void SpeedControl()
    {
        // Limit Speed on Slope
        if(OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        // Limiting Speed on ground or in air
        else
        {
            // Getting the flat velocity of the rigidbody
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // If this velocity exceeds the moveSpeed, limit the speed
            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    // Handles the player's ability to Jump
    private void Jump()
    {
        exitingSlope = true;

        // Reset the player's Y Velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Add jumpForce Once
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Reset the player's ability to jump
    private void ResetJump()
    {
        canJump = true;

        exitingSlope = false;
    }

    // Check if the player is on a slope
    private bool OnSlope() 
    {
        // Use a raycast to determine if the player is on a sloped surface
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            // Return true if the player is on a sloped surface with an angle less than the maxSlopeAngle, but not 0
            return angle < maxSlopeAngle && angle != 0;
        }
        // If nothing is hit, return false.
        return false;
    }

    // Project normal movement direction to the given slope
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    // Detects if the player is in a Spawn Area or End Area
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the spawn area
        if(other.gameObject.CompareTag("SpawnArea"))
        {
            isInSpawnArea = true;
        }

        // Check if the player entered the end area
        if(other.gameObject.CompareTag("EndArea"))
        {
            isInEndArea = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Check if the player exited the spawn area
        if(other.gameObject.CompareTag("SpawnArea"))
        {
            isInSpawnArea = false;
        }

        // Check if the player exited the end area
        if(other.gameObject.CompareTag("EndArea"))
        {
                isInEndArea = false;
        }
    }
}
