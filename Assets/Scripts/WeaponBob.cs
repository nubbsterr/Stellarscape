using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    // references to control when and how weapon bob should be displayed
    public PlayerMovement PlayerState;
    public ADS ADSed;

    private bool isWalking;
    private bool isRunning;
    private bool isGrounded;
    private bool Sliding;
    private bool isAiming;

    [Range(0.001f,0.01f)]
    public float curveSmoothness = 0.002f;
    [Range(1f,30f)] // basically gives us sliders for variables now!!
    public float Frequency = 10f;
    [Range(10f,100f)]
    public float curveSpeed = 10f;

    Vector3 StartPos;
    
    // Start is called before the first frame update
    void Start()
    {
        StartPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        isAiming = ADSed.fullADS; // disable weapon bob if ADSed
        isWalking = !PlayerState.isSprinting; // not sprinting means we must be walking lol
        isRunning = PlayerState.isSprinting;
        isGrounded = PlayerState.isGrounded;
        Sliding = PlayerState.isSliding;

        StopWeaponBob();
        WeapomBobTrigger();
    }

    private void WeapomBobTrigger()
    {
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude; // basically a measure of whether or not we're moving at all
        if (inputMagnitude > 0 && (isWalking || isRunning) && isGrounded && !Sliding && !isAiming) // basically do the bobbing only if we're running or walking, grounded, and not ADSed
        {
            StartWeaponBob();
        }
    }

    private Vector3 StartWeaponBob()
    {
        Vector3 pos = Vector3.zero;
        
        if (isWalking)
        {
            pos.y = Mathf.Lerp(pos.y, Mathf.Sin(Time.time * Frequency) *curveSmoothness * 1.4f, curveSpeed * Time.deltaTime);
            pos.x = Mathf.Lerp(pos.x, Mathf.Sin(Time.time * Frequency/2f) *curveSmoothness * 1.6f, curveSpeed * Time.deltaTime);
            transform.localPosition += pos; // shift around our weapons effectlvely
        }
        if (isRunning)
        {
            pos.y = Mathf.Lerp(pos.y, Mathf.Sin(Time.time * Frequency) *curveSmoothness * 1.4f, (curveSpeed * 1.7f) * Time.deltaTime);
            pos.x = Mathf.Lerp(pos.x, Mathf.Sin(Time.time * Frequency/2f) *curveSmoothness * 1.6f, (curveSpeed * 1.9f) * Time.deltaTime);
            transform.localPosition += pos; // shift around our weapons effectlvely
        }
        return pos;
    }

    private void StopWeaponBob()
    { // bring the weapon back to the start position at all times
        if (transform.localPosition == StartPos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, StartPos, 1 * Time.deltaTime);
    }
}
