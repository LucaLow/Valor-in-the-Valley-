using System;
using System.Collections;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public CameraController cameraController;
    // This transforms initial position
    private Vector3 defaultPosition;

    [Space(10)]

    public HeadBobSettings headBobSettings;
    [Space]
    public LookTiltSettings lookTiltSettings;
    [Space]
    public HorizontalTiltDueToVelocitySettings horizontalTiltDueToVelocitySettings;
    [Space]
    public VerticalTiltDueToVelocitySettings verticalTiltDueToVelocitySettings;
    [Space]
    public MovementFOVAdjustmentsSettings movementFOVAdjustmentsSettings;

    private float lookTilt = 0;
    private float moveTilt = 0;
    private float tilt;

    private bool FOVBlocked = false;

    private void Start()
    {
        defaultPosition = transform.localPosition;

        // mess with FOV depending on the users settings.
        movementFOVAdjustmentsSettings.sprintingFOV = 70f * StaticVariables.CameraFOV / 85;
        movementFOVAdjustmentsSettings.movingForwardFOV = 63f * StaticVariables.CameraFOV / 85;
        movementFOVAdjustmentsSettings.movingBackwardFOV = 57f * StaticVariables.CameraFOV / 85;
        movementFOVAdjustmentsSettings.idleFOV = 60f * StaticVariables.CameraFOV / 85;
}

    private void Update()
    {
        if (PlayerController.active == false)
            return;

        tilt = 0;

        if (PauseManager.IsPaused == false)
        {
            if (headBobSettings.enabled)
                PerformHeadBob();
            if (lookTiltSettings.enabled)
                PerformLookTilt();
            if (horizontalTiltDueToVelocitySettings.enabled)
                PerformHorizontalTilt();
            if (verticalTiltDueToVelocitySettings.enabled)
                PerformVerticalTilt();
        }
        if (movementFOVAdjustmentsSettings.enabled && FOVBlocked == false)
            PerformFOVAdjustments();

        ApplyTilt();
    }

    private void LateUpdate()
    {
        FOVBlocked = false;
    }

    /// <summary>
    /// Stops forcing the cameras field of view for this frame
    /// </summary>
    public void TemporarilyStopFOV()
    {
        FOVBlocked = true;
    }

    public void ShakeCamera(float intensity = 0.5f, float speed = 20f, float length = 0.25f)
    {
        StartCoroutine(CameraShake(intensity, speed, length));
    }

    private IEnumerator CameraShake(float intensity, float speed, float length)
    {
        float timer = length;
        Vector3 defaultPosition = transform.localPosition;

        while (true)
        {
            // Use perlin noise to shake camera
            float noiseX = Mathf.PerlinNoise(Time.time * 0.5f * speed, Time.time * speed);
            float noiseY = Mathf.PerlinNoise(Time.time * speed, Time.time * 0.5f * speed);

            // Allow for negative noise values
            float posX = noiseX * 2 - 1;
            float posY = noiseY * 2 - 1;
            Vector2 offset = new Vector2(posX, posY) * intensity;

            // Apply the rotation
            transform.localPosition = Vector2.Lerp(transform.localPosition, (Vector2)defaultPosition + offset, 10 * Time.unscaledDeltaTime);

            yield return new WaitForEndOfFrame();

            // Limit the time to shake camera
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0)
            {
                // Reset the camera position to the default
                while (Mathf.Abs(defaultPosition.x - transform.localPosition.x) > 0.001f)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, defaultPosition, 10 * Time.unscaledDeltaTime);
                    yield return new WaitForEndOfFrame();
                }
                transform.localPosition = defaultPosition;
                break;
            }
        }
    }

    public void PerformHeadBob()
    {
        // Calculate a sin wave for the horizontal and vertical head bob
        float verticalSinWave = Mathf.Sin(Time.timeSinceLevelLoad * headBobSettings.frequency * playerMovement.speed);
        float horizontalSinWave = Mathf.Sin(Time.timeSinceLevelLoad * headBobSettings.frequency * playerMovement.speed * 0.5f);

        // Create a value for the horizontal and vertical head bobs
        float verticalHeadBob = verticalSinWave * headBobSettings.verticalIntensity * 0.1f;
        float horizontalHeadBob = horizontalSinWave * headBobSettings.horizontalIntensity * 0.1f;

        // Create a vector from the head bob values
        Vector3 position = transform.localPosition;
        position.x = Mathf.Lerp(position.x, defaultPosition.x + horizontalHeadBob, headBobSettings.interpolationSpeed * Time.unscaledDeltaTime);
        position.y = Mathf.Lerp(position.y, defaultPosition.y + verticalHeadBob, headBobSettings.interpolationSpeed * Time.unscaledDeltaTime);

        // Apply the head bob
        transform.localPosition = position;
    }

    private void PerformLookTilt()
    {
        // Get the horizontal mouse input
        double horizontal = (double) -cameraController.GetHorizontalMouseInput() / Time.unscaledDeltaTime;
        // Multiply the mouse input by the rotation intensity
        double rotation = horizontal * lookTiltSettings.rotationIntensity * 0.01f;

        lookTilt = Mathf.Lerp(lookTilt, (float) rotation, lookTiltSettings.rotationInterpolationSpeed * Time.unscaledDeltaTime);
    }

    private void PerformHorizontalTilt()
    {
        float horizontalMovement = -PlayerMovement.GetMovementInput().x;
        float targetTilt = horizontalMovement * horizontalTiltDueToVelocitySettings.rotationIntensity;

        moveTilt = Mathf.LerpAngle(moveTilt, targetTilt, horizontalTiltDueToVelocitySettings.rotationInterpolationSpeed * Time.unscaledDeltaTime);
    }

    private void PerformVerticalTilt()
    {
        // Get the vertical velocity of the player
        float verticalVelocity = playerMovement.trueVelocity.y;
        // Multiply the velocity by the rotation intensity
        float rotation = verticalVelocity * verticalTiltDueToVelocitySettings.rotationIntensity;

        rotation = Mathf.Clamp(rotation, verticalTiltDueToVelocitySettings.limitAngle.x, verticalTiltDueToVelocitySettings.limitAngle.y);

        // Get a reference to the current transforms angle
        Vector3 eulerAngles = transform.localEulerAngles;
        // Interpolate the angle along the x axis to the new rotation
        eulerAngles.x = Mathf.LerpAngle(eulerAngles.x, rotation, verticalTiltDueToVelocitySettings.rotationInterpolationSpeed * Time.unscaledDeltaTime);

        if (eulerAngles.magnitude.ToString() == "NaN")
            return;

        // Apply the rotation
        transform.localEulerAngles = eulerAngles;
    }

    private void PerformFOVAdjustments()
    {
        Camera playerCamera = playerMovement.playerCamera;

        // Camera FOV
        if (playerMovement.speed == playerMovement.movement.sprintSpeed)
        {
            // Sprinting
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, movementFOVAdjustmentsSettings.sprintingFOV, movementFOVAdjustmentsSettings.sprintFOVChangeTime * Time.unscaledDeltaTime);
        }
        else
        {
            if (PlayerMovement.GetMovementInput().y > 0 && PauseManager.IsPaused == false)
                // Moving forwards
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, movementFOVAdjustmentsSettings.movingForwardFOV, movementFOVAdjustmentsSettings.regularFOVChangeTime * Time.unscaledDeltaTime);
            else if (PlayerMovement.GetMovementInput().y < 0)
                // Moving backwards
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, movementFOVAdjustmentsSettings.movingBackwardFOV, movementFOVAdjustmentsSettings.regularFOVChangeTime * Time.unscaledDeltaTime);
            else
                // Not moving
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, movementFOVAdjustmentsSettings.idleFOV, movementFOVAdjustmentsSettings.regularFOVChangeTime * Time.unscaledDeltaTime);
        }
    }

    private void ApplyTilt()
    {
        if (PauseManager.IsPaused) return;

        tilt = lookTilt + moveTilt;

        // Get a reference to the current transforms angle
        Vector3 eulerAngles = transform.localEulerAngles;
        // Apply the tilt
        eulerAngles.z = tilt;

        // Apply the rotation
        transform.localEulerAngles = eulerAngles;
    }
}

[System.Serializable]
public class HeadBobSettings
{
    public bool enabled = true;

    [Space]

    public float interpolationSpeed = 7f;
    public float frequency = 4f;
    public float verticalIntensity = 0.75f;
    public float horizontalIntensity = 0.35f;
}


[System.Serializable]
public class LookTiltSettings
{
    public bool enabled = true;

    [Space]

    public float rotationIntensity = 3f;
    public float rotationInterpolationSpeed = 10f;
}

[System.Serializable]
public class HorizontalTiltDueToVelocitySettings
{
    public bool enabled = true;
    
    [Space]

    public float rotationIntensity = 5f;
    public float rotationInterpolationSpeed = 5f;
}

[System.Serializable]
public class VerticalTiltDueToVelocitySettings
{
    public bool enabled = true;
    
    [Space]

    public Vector2 limitAngle = new Vector2(-10f, 10f);
    public float rotationIntensity = 1f;
    public float rotationInterpolationSpeed = 10f;
}

[System.Serializable]
public class MovementFOVAdjustmentsSettings
{
    public bool enabled = true;

    [Space]

    public float regularFOVChangeTime = 5f;
    public float sprintFOVChangeTime = 10f;

    [Space]

    public float sprintingFOV = 70f;
    public float movingForwardFOV = 63f;
    public float movingBackwardFOV = 57f;
    public float idleFOV = 60f;
}