using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float playerSpeed = 10f;
    float sprintMultiplier = 2f;
    void Update()
    {
        Vector3 movement = Vector3.zero; // movement is set to Zero by default, at rest.

    // Check for key presses
    if (Input.GetKey(KeyCode.W))
    {
        movement += Vector3.forward;
    }
    if (Input.GetKey(KeyCode.S))
    {
        movement += Vector3.back;
    }
    if (Input.GetKey(KeyCode.A))
    {
        movement += Vector3.left;
    }
    if (Input.GetKey(KeyCode.D))
    {
        movement += Vector3.right;
    }
    if (Input.GetKey(KeyCode.LeftShift))
    {
        movement = movement*sprintMultiplier; // multiply our current movement values by the sprint multiplier (increase movement speed by 2)
    }
    // Apply speed and time scaling
    Vector3 moveSpeed = movement * playerSpeed * Time.deltaTime;

    // Apply movement
    transform.Translate(moveSpeed);
    }
}
