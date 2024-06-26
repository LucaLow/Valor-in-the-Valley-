using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement playerMovementScript;
    public CameraController cameraControllerScript;

    [Space]

    public Camera playerCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        cameraControllerScript.RotateCamera(playerCamera);
        cameraControllerScript.RotateBody(transform);

        playerMovementScript.ApplyGravity();
        playerMovementScript.Move();
        playerMovementScript.CheckForJumps();
        playerMovementScript.CheckForCrouch();
        playerMovementScript.CheckForLean();
    }
}
