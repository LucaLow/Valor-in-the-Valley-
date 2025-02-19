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
    private Coroutine clickLoop;

    private const int idleHash = 1870961784;

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (PauseManager.IsPaused) return;

            clickLoop = StartCoroutine(LoopClicks());
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (clickLoop != null)
                StopCoroutine(clickLoop);
            clickLoop = null;
        }

        if (isPlayingAnimation == false && string.IsNullOrEmpty(queuedAnimation) == false)
        {
            // Queued animation may be played now.

            //print("Setting trigger: " + queuedAnimation);
            animator.SetTrigger(queuedAnimation);
            queuedAnimation = "";
        }
    }

    private IEnumerator LoopClicks()
    {
        int currentHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        if (BuildManager.currentPreview == null) // Not building
        {
            if (currentHash == idleHash && isPlayingAnimation == false)
            {
                InitiateAnimation();

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

        yield return new WaitForSeconds(0.2f);

        if (Input.GetMouseButton(0))
        {
            clickLoop = StartCoroutine(LoopClicks());
        }
        else
        {
            clickLoop = null;
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
        float intensity = 0.5f;
        float speed = 8f;
        float length = 0.15f;

        cameraEffects.ShakeCamera(intensity, speed, length);
    }
}