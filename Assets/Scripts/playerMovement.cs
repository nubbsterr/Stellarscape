using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // refers to our stance images displayed on the UI, active depending on movement by player
    public GameObject sprintingStance;
    public GameObject walkingStance;
    public GameObject standingStance;
    public GameObject jumpingStance;
    public GameObject slidingStance;

    public float playerSpeed = 10f;
    public float playerHeight; // this is just for setting up the raycasting so we have a point to shoot from
    public float groundDrag = 1.0f; // the amount of drag set to the player when grounded
    
    public float jumpForce; // force multiplier to increase jump height
    public float airMultiplier; // movement multiplier if airborne

    public float slidingDrag = 3f; // float value for drag set to the player on slide command
    public float SlideDuration = 2f; // duration of slide in-game

    float sprintMultiplier = 1.15f;
    float horizontalInput;
    float verticalInput;
    
    public bool isSprinting;
    
    public bool isGrounded = false; // disable/enable jumping based on
    bool jumpReady = true; // disables/enables jump based on cooldown set

    bool slidingReady; // disables/enables sliding mechanic based on cooldown and current movement oritentation of player
    public bool isSliding; // disables movement while sliding, allows us to use it in other scripts as well
    bool cancelSlide = false; // cancel slide immediately by exiting coroutine
    
    private Vector3 slidingVelocity; // vector value of our velocity before sliding, applied during slide
    
    Rigidbody rb; // acquiring rigidbody of our player

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        Jump(); // jump functionality
        Slide(); // slide functionality
        
        // condition to cancel slide movement when sliding
        cancelSlide = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        
        ControlSpeed(isSprinting); // control player speed
        ShowStance(); // show player stance/movement status on UI display

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput =Input.GetAxis("Vertical");
        
        Ray playerpos = new Ray(transform.position, Vector3.down);
        RaycastHit rayhit; // basically true/false if the ray hits something

        if(Physics.Raycast(playerpos, out rayhit, playerHeight/2 + 0.4f)) 
        {
            if (rayhit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
                slidingReady = true;
                rb.drag = groundDrag; // set drag to the player!

                Vector3 movement = transform.forward * verticalInput + transform.right * horizontalInput;
                if (!isSliding)
                {
                    rb.AddForce(movement.normalized * playerSpeed, ForceMode.Force);
                }
                if (isSliding)
                {
                    slidingVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                    rb.AddForce(slidingVelocity.normalized * 0.25f, ForceMode.Impulse); // slow but fast movement while sliding based on previous movement
                }
                isSprinting = false; // if not sprinting, sprinting is false otherwise while grounded
        
                if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1) && isGrounded && !isSliding) // holding down shift and NOT ADSED (don't wanna run while ADSed lel), must be grounded to sprint and not sliding
                {
                    movement = transform.forward * verticalInput + transform.right * horizontalInput;
                    rb.AddForce(movement.normalized * playerSpeed * sprintMultiplier, ForceMode.Force);
                    isSprinting = true;
                }
            }
        }
        else // airborne movement
        {
            Vector3 movement = transform.forward * verticalInput + transform.right * horizontalInput;

            isGrounded = false;
            slidingReady = false;
            rb.drag = 0;

            rb.AddForce(movement.normalized * playerSpeed * airMultiplier, ForceMode.Force);
        }
    }
    private void Jump()
    {
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumpReady)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset velocity to have a consistent jump height/force

                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                StartCoroutine(jumpCooldown()); // start jump cooldown and disable jumping for a bit to avoid wacky movement
            }
        }
    }
    private IEnumerator jumpCooldown()
    {
        jumpReady = false;

        yield return new WaitForSeconds(3f);

        jumpReady = true;
    }

    private void Slide()
    {
        if (isGrounded && verticalInput > 0) // moving forward and grounded
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) && !isSliding && slidingReady && !Input.GetMouseButtonDown(1)) // aren't already sliding and aren't ADSed
            {
                Debug.Log("sliding");
                isSliding = true;
                jumpReady = false; // cannot jump while sliding lol
                
                rb.drag = slidingDrag; // lower player drag

                StartCoroutine(StopSlide());
            }
        }
    }

    private IEnumerator StopSlide()
    {
        cancelSlide = false; // set to false before anything happense to avoid errors

        float elapsedTime = 0f;

        while (elapsedTime < SlideDuration)
        {
            if (cancelSlide) // player tries cancelling slide
            {
                Debug.LogWarning("Slide cancelled");
                
                // resetting all vars
                isSliding = false;
                slidingReady = true;
                jumpReady = true;

                // immediately exit the coroutine
                yield break;
            }
            elapsedTime += Time.deltaTime; // increase time naturally
            yield return null;
        }
        if (!cancelSlide)
        {
            slidingReady = false; // disable sliding during slide action
            yield return new WaitForSeconds(SlideDuration);
            rb.drag = groundDrag;
            
            slidingReady = true;
            isSliding = false;
            jumpReady = true;
        }
    }
    private void ControlSpeed(bool isSprinting)
    {
        Vector3 cappedVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // limit player speed based on cappedVelocity magnitude
        float maxSpeed = isSprinting ? playerSpeed * sprintMultiplier : playerSpeed; // max possible speed depending on if the player is sprinting or not
        if (cappedVelocity.magnitude > maxSpeed)
        {
            Vector3 cappedSpeed = cappedVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(cappedSpeed.x,rb.velocity.y,cappedSpeed.z); // limit speed on ground level
        }
    }

    private void ShowStance() // show player stance/movement on screen through images on UI display
    {
        if (horizontalInput == 0 && verticalInput == 0)
        {
            standingStance.SetActive(true);
            
            walkingStance.SetActive(false);
            sprintingStance.SetActive(false);
            slidingStance.SetActive(false);
            jumpingStance.SetActive(false);
        }
        if (horizontalInput != 0 || verticalInput != 0 && isSprinting) // we're sprinting
        {
            sprintingStance.SetActive(true);
            
            walkingStance.SetActive(false);
            standingStance.SetActive(false);
            slidingStance.SetActive(false);
            jumpingStance.SetActive(false);
        }
        if (horizontalInput != 0 || verticalInput != 0 && !isSprinting) // not sprinting but moving
        {
            walkingStance.SetActive(true);
            
            sprintingStance.SetActive(false);
            standingStance.SetActive(false);
            slidingStance.SetActive(false);
            jumpingStance.SetActive(false);
        }
        if (!isGrounded) // i.e. we're jumping/airborne
        {
            jumpingStance.SetActive(true);

            walkingStance.SetActive(false);
            sprintingStance.SetActive(false);
            standingStance.SetActive(false);
            slidingStance.SetActive(false);
        }
        if (isSliding) // self explanatory enough
        {
            slidingStance.SetActive(true);

            jumpingStance.SetActive(false);
            walkingStance.SetActive(false);
            sprintingStance.SetActive(false);
            standingStance.SetActive(false);
        }
    }
}