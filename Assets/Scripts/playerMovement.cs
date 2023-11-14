using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float playerSpeed = 10f;
    float sprintMultiplier = 2f;

    float horizontalInput;
    float verticalInput;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput =Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * verticalInput + transform.right * horizontalInput;
        rb.AddForce(movement.normalized * playerSpeed, ForceMode.Force);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement = transform.forward * verticalInput + transform.right * horizontalInput;
            rb.AddForce(movement.normalized * playerSpeed * sprintMultiplier, ForceMode.Force);
        }
    }
}
