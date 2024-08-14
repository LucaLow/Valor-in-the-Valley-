using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public PlayerController playerController;
    public CameraEffects cameraEffects;
    public Animator animator;

    public string[] fromRightAnimations;
    public string[] fromLeftAnimations;
    public string nextDirection = "From Left";

    private string queuedAnimation = "";
    private bool isPlayingAnimation = false;

    private const int idleHash = 1870961784;

    private void Update()
    {
        int currentHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        if (Input.GetMouseButtonDown(0)) // Left click to attack
        {
            if (playerController.playerMovementScript.speed != playerController.playerMovementScript.movement.sprintSpeed) // Not sprinting
            {
                if (BuildManager.currentPreview == null) // Not building
                {
                    if (currentHash == idleHash && isPlayingAnimation == false)
                    {
                        // Animation is not playing so start a swing animation

                        // Select a random animation to play
                        int firstSwingIndex = Random.Range(0, fromRightAnimations.Length);
                        string firstSwing = fromRightAnimations[firstSwingIndex];

                        // Play the random animation
                        animator.SetTrigger(firstSwing);

                        // Adjust the direction for any consecutively played animations
                        nextDirection = "left";
                    }
                    else
                    {
                        if (isPlayingAnimation && string.IsNullOrEmpty(queuedAnimation))
                        {
                            if (nextDirection == "left")
                            {
                                // Select a random animation to play
                                int swingIndex = Random.Range(0, fromLeftAnimations.Length);
                                string swing = fromLeftAnimations[swingIndex];

                                // Queue the swing animation
                                queuedAnimation = swing;
                            }
                            else if (nextDirection == "right")
                            {
                                // Select a random animation to play
                                int swingIndex = Random.Range(0, fromRightAnimations.Length);
                                string swing = fromRightAnimations[swingIndex];

                                // Queue the swing animation
                                queuedAnimation = swing;
                            }

                            // Alternate swing direction
                            nextDirection = nextDirection == "left" ? "right" : "left";
                        }
                    }
                }
            }
        }

        if (isPlayingAnimation == false && string.IsNullOrEmpty(queuedAnimation) == false)
        {
            // Queued animation may be played now.

            //print("Setting trigger: " + queuedAnimation);
            animator.SetTrigger(queuedAnimation);
            queuedAnimation = "";
        }
    }

    public void InitiateAnimation()
    {
        isPlayingAnimation = true;
    }

    public void EndAnimation()
    {
        isPlayingAnimation = false;
    }

    public void ShakeCamera()
    {
        float intensity = .25f;
        float speed = 5f;
        float length = 0.15f;

        cameraEffects.ShakeCamera(intensity, speed, length);
    }
}