using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;

    [Header("Physics")]
    public float gravity = -9.8f;
    private float yVelocity = 0f;

    public Vector3 trueVelocity { get; private set; }

    [Header("Grounding")]
    public LayerMask groundLayer;
    public Vector3 groundCheck;
    public float groundCheckRadius = 0.1f;

    [Header("Roofing")]
    public LayerMask roofLayer;
    public Vector3 roofCheck;
    public float roofCheckRadius = 0.1f;

    [Space(20)]

    public PlayerMovementSettings movement;
    public float speed { get; private set; }
    [Space]
    public PlayerJumpingSettings jumping;
    private int additionalJumpsUsed = 0;
    [Space]
    public PlayerCrouchSettings crouching;
    private float defaultControllerSize;
    private bool isCrouching = false;
    [Space]
    public PlayerLeaningSettings leaning;
    private int leanDirection = 0; // -1 = lean left, 0 = no lean, 1 = lean right



    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        defaultControllerSize = characterController.height;

        StartCoroutine(UpdateTrueVelocity());
    }

    private void Update()
    {
        if (IsGrounded())
        {
            additionalJumpsUsed = 0;
        }
    }

    public void Move()
    {
        float horizontal = GetMovementInput().x;
        float vertical = GetMovementInput().y;

        speed = movement.walkSpeed;

        if (movement.canSprint && Input.GetKey(movement.sprintKey))
        {
            if (!(isCrouching && IsRoofed()))
            {
                if (vertical > 0)
                {
                    speed = movement.sprintSpeed;
                }
                
            }
        }


        if (isCrouching)
        {
            speed = crouching.crouchSpeed;
            characterController.height = Mathf.Lerp(characterController.height, crouching.crouchColliderSize, 20 * Time.deltaTime);
        }
        else
        {
            characterController.height = Mathf.Lerp(characterController.height, defaultControllerSize, 20 * Time.deltaTime);
        }


        // Not moving so reset speed
        if (horizontal == 0 && vertical == 0)
        {
            speed = 0;
        }

        characterController.Move((transform.forward * vertical + transform.right * horizontal).normalized * speed * Time.deltaTime);
    }

    public static Vector2 GetMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        return new Vector2(horizontal, vertical);
    }

    private IEnumerator UpdateTrueVelocity()
    {
        Vector3 wasPosition = transform.position;

        yield return new WaitForEndOfFrame();

        trueVelocity = (transform.position - wasPosition) / Time.deltaTime;

        StartCoroutine(UpdateTrueVelocity());
    }

    public void ApplyGravity()
    {
        if (IsGrounded() == false)
            yVelocity += gravity * Time.deltaTime;

        characterController.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
    }

    public void CheckForJumps()
    {
        if (jumping.enabled == false || leanDirection != 0)
            return;

        if (Input.GetKeyDown(jumping.jumpKey))
        {
            if (IsGrounded())
            {
                // Jump
                yVelocity = jumping.jumpSpeed;
            }
            else
            {
                if (additionalJumpsUsed < jumping.additionalJumps)
                {
                    // Jump
                    yVelocity = jumping.additionalJumpSpeed;
                    additionalJumpsUsed++;
                }
            }
        }
    }

    public void CheckForCrouch()
    {
        if (crouching.enabled == false)
            return;

        if (IsGrounded())
        {
            // Holding to crouch
            if (crouching.holdToCrouch)
            {
                if (Input.GetKey(crouching.crouchKey))
                {
                    isCrouching = true;
                }
                else
                {
                    if (IsRoofed())
                        return;

                    isCrouching = false;
                }
            }
            // Toggle crouching
            else
            {
                if (Input.GetKeyDown(crouching.crouchKey))
                {
                    if (isCrouching)
                    {
                        if (IsRoofed())
                            return;

                        isCrouching = false;
                    }
                    else
                    {
                        isCrouching = true;
                    }
                }
            }

            // Crouch cancelled by jumping/sprinting
            if ((Input.GetKeyDown(jumping.jumpKey) || Input.GetKey(movement.sprintKey)) && IsRoofed() == false)
            {
                isCrouching = false;
            }

            // Adjust collider size to crouch
            if (isCrouching)
            {
                yVelocity = gravity * 0.5f;
                //characterController.height = crouching.crouchColliderSize;
            }
            else
            {
                //characterController.height = defaultControllerSize;
            }
        }
    }

    public void CheckForLean()
    {
        if (leaning.enabled == false)
            return;

        if (leaning.holdToLean)
        {
            if (Input.GetKey(leaning.leanLeft))
                leanDirection = -1;
            if (Input.GetKeyUp(leaning.leanLeft))
                leanDirection = 0;
            if (Input.GetKey(leaning.leanRight))
                leanDirection = 1;
            if (Input.GetKeyUp(leaning.leanRight))
                leanDirection = 0;
            
            // Both lean keys are pressed
            if (Input.GetKey(leaning.leanLeft) && Input.GetKey(leaning.leanRight))
                leanDirection = 0;
        }
        else
        {
            if (Input.GetKeyDown(leaning.leanLeft))
            {
                if (leanDirection == -1)
                    leanDirection = 0;
                else
                    leanDirection = -1;
            }
            if (Input.GetKeyDown(leaning.leanRight))
            {
                if (leanDirection == 1)
                    leanDirection = 0;
                else
                    leanDirection = 1;
            }
        }

        if (speed == movement.sprintSpeed)
        {
            leanDirection = 0;
        }

        // Rotate camera
        Vector3 cameraRotation = playerCamera.transform.localEulerAngles;
        cameraRotation.z = Mathf.LerpAngle(cameraRotation.z, leaning.leanAngle * -leanDirection, leaning.leanInterpolationSpeed * Time.deltaTime);
        playerCamera.transform.localEulerAngles = cameraRotation;

        // Offset camera
        Vector3 cameraPosition = playerCamera.transform.localPosition;
        cameraPosition.x = Mathf.Lerp(cameraPosition.x, leaning.leanOffset * leanDirection, leaning.leanInterpolationSpeed * Time.deltaTime);
        playerCamera.transform.localPosition = cameraPosition;
    }

    public bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position + groundCheck, groundCheckRadius, groundLayer))
        {
            return true;
        }
        return false;
    }
    public bool IsRoofed()
    {
        if (Physics.CheckSphere(transform.position + roofCheck, roofCheckRadius, roofLayer))
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + roofCheck, roofCheckRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position + groundCheck, groundCheckRadius);
    }
}

[System.Serializable]
public class PlayerMovementSettings
{
    public float walkSpeed = 4f;

    [Space]

    public bool canSprint = true;
    public float sprintSpeed = 6f;
    public KeyCode sprintKey = KeyCode.LeftShift;
}

[System.Serializable]
public class PlayerJumpingSettings
{
    public bool enabled = true;

    [Space]

    public float jumpSpeed = 4f;
    public float additionalJumpSpeed = 3f;
    public int additionalJumps = 0;
    public KeyCode jumpKey = KeyCode.Space;
}

[System.Serializable]
public class PlayerCrouchSettings
{
    public bool enabled = true;

    [Space]

    public bool holdToCrouch = false;
    public float crouchSpeed = 2f;
    public float crouchColliderSize = 0.75f;
    public KeyCode crouchKey = KeyCode.LeftControl;
}

[System.Serializable]
public class PlayerLeaningSettings
{
    public bool enabled = false;

    [Space]

    public bool holdToLean = false;

    public float leanAngle = 15;
    public float leanOffset = 0.3f;
    public float leanInterpolationSpeed = 7f;

    public KeyCode leanLeft = KeyCode.Q;
    public KeyCode leanRight = KeyCode.E;
}