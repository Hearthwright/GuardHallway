using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed; // Determines how fast the player is able to move
    public float groundDrag; // Determines the amount of drag experienced whilst grounded

    [Header("Jump Settings")]
    public float jumpForce; // Determines the amount of force applied when jumping
    public float jumpCooldown; // Determines the amount of time between jumps
    public float airMultiplier; // Determines how speed is effected while airborne
    bool canJump; // Determines if the player is currently allowed to jump

    [Header("Keybind Settings")]
    public KeyCode jumpKey = KeyCode.Space; // Determines which key handles jumping (Default = Space)

    [Header("Ground Check")]
    public float playerHeight; // Stores the player's height
    public LayerMask whatIsGround; // Determines what surfaces are the ground
    bool grounded; // Determines if the player is currently in a grounded state

    public Transform orientation; // Handles the player's current orientation

    float horizontalInput; // Determines the player's current Horizontal Input
    float verticalInput; // Determines the player's current Vertical Input

    Vector3 moveDirection; // Determines the direction the player is moving in

    Rigidbody rb; // Reference to the player's rigidbody


    // Start is called before the first frame update
    void Start()
    {
        // Assign the rigidbody and freeze its rotation
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        canJump = true;
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
    }

    // Limits player Speed
    private void SpeedControl()
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

    // Handles the player's ability to Jump
    private void Jump()
    {
        // Reset the player's Y Velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Add jumpForce Once
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Reset the player's ability to jump
    private void ResetJump()
    {
        canJump = true;
    }
}
