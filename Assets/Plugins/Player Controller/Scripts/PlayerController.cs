using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static bool active = true;

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
        playerMovementScript.ApplyGravity();

        if (active == false)
            return;

        cameraControllerScript.RotateCamera(playerCamera);
        cameraControllerScript.RotateBody(transform);

        playerMovementScript.Move();
        playerMovementScript.CheckForJumps();
        playerMovementScript.CheckForCrouch();
        playerMovementScript.CheckForLean();
    }
}
