using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Animator animator;

    public string animation1;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (BuildManager.currentPreview == null)
            {
                animator.Play(animation1);

            }
        }
    }
}
