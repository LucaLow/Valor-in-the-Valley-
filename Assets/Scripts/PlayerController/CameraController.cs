using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float horizontalSensitivity = 1f;
    public float verticalSensitivity = 1f;

    public float interpolationSpeed = 0f;

    [Space]

    public Vector2 clamp = new Vector2(-70f, 80f);

    [Space]

    private float horizontalRotation = 0f;
    private float interpolatedHorizontalRotation = 0f;
    private float verticalRotation = 0f;
    private float interpolatedVerticalRotation = 0f;

    [Space]

    public string horizontalInput = "Mouse X";
    public string verticalInput = "Mouse Y";

    public void Start()
    {
        horizontalSensitivity = StaticVariables.MouseSensitivity/1.5f; 
        verticalSensitivity = StaticVariables.MouseSensitivity/1.5f;
    }

    public void RotateCamera(Camera camera)
    {
        // Get the mouse input
        float verticalMouseInput = GetVerticalMouseInput();
        // Get the rotation value
        float verticalRotate = verticalMouseInput * verticalSensitivity;

        // This is the final, unsmoothed rotation
        verticalRotation -= verticalRotate;

        // Clamp the rotation
        verticalRotation = Mathf.Clamp(verticalRotation, clamp.x, clamp.y);

        // Calculate the smoothed rotation
        interpolatedVerticalRotation = Mathf.LerpAngle(interpolatedVerticalRotation, verticalRotation, interpolationSpeed * Time.deltaTime);

        // The final rotation vector
        Vector3 finalRotation = camera.transform.localEulerAngles;

        // Check if smoothing is enabled
        if (interpolationSpeed <= 0)
            finalRotation.x = verticalRotation;
        else
            finalRotation.x = interpolatedVerticalRotation;


        // Apply the rotation
        camera.transform.localEulerAngles = finalRotation;
    }


    public void RotateBody(Transform body)
    {
        // Get the mouse input
        float horizontalMouseInput = GetHorizontalMouseInput();
        // Get the rotation value
        float horizontalRotate = horizontalMouseInput * horizontalSensitivity;

        // This is the final, unsmoothed rotation
        horizontalRotation += horizontalRotate;

        // Calculate the smoothed rotation
        interpolatedHorizontalRotation = Mathf.LerpAngle(interpolatedHorizontalRotation, horizontalRotation, interpolationSpeed * Time.deltaTime);

        // The final rotation vector
        Vector3 finalRotation = body.localEulerAngles;

        // Check if smoothing is enabled
        if (interpolationSpeed <= 0)
            finalRotation.y = horizontalRotation;
        else
            finalRotation.y = interpolatedHorizontalRotation;


        // Apply the rotation
        body.localEulerAngles = finalRotation;
    }

    public float GetHorizontalMouseInput()
    {
        return Input.GetAxisRaw(horizontalInput);
    }
    
    public float GetVerticalMouseInput()
    {
        return Input.GetAxisRaw(verticalInput);
    }
}
