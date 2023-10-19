using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerJumping : MonoBehaviour
{
    public float jumpForce = 10.0f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Assign the Rigidbody component
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) // Check if grounded before jumping
        {
            Vector3 jump = new Vector3(0, jumpForce, 0);
            rb.AddForce(jump, ForceMode.Impulse);
        }
    }

    bool IsGrounded() // Declare the method here, outside of Update
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.1f);
    }

}

