using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float sensitivity = 5.0f;
    private float forwardInputValue;
    private float strafeInputValue;
    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        characterController.SimpleMove(Vector3.forward * 0);
        forwardInputValue = Input.GetAxisRaw("Vertical");
        strafeInputValue = Input.GetAxisRaw("Horizontal");
        float mouseX = Input.GetAxis("Mouse X");
        transform.eulerAngles += new Vector3(0, mouseX * sensitivity);
        Movement();
    }

    void Movement()
    {
        Vector3 direction = (transform.forward * forwardInputValue
                           + transform.right * strafeInputValue).normalized
                           * movementSpeed * Time.deltaTime;

        characterController.Move(direction);
                               
    }
}
