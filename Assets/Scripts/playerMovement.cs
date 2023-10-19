using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float playerSpeed = 10f;
    float sprintMultiplier = 2f;
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
        movement = movement.normalized * playerSpeed * Time.deltaTime;

        // Check if Shift key is held down for sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement *= sprintMultiplier;
            Debug.Log("Sprinting");
        }
        // Normalize the movement vector before applying speed
        movement.Normalize();

        // Apply movement speed
        
        Vector3 moveSpeed = movement * playerSpeed * Time.deltaTime;
        transform.Translate(movement);
    }
}
