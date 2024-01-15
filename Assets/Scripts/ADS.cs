using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class ADS : MonoBehaviour
{
    public GameObject crosshair; // refers to our crosshair/center dot on screen
    private Vector3 ADSpos;
    private Vector3 farADSpos;
    
    // both vars store the position of the gun models when not being ADSed
    public Transform primaryPos;
    private Vector3 primaryStartPos;
    public Transform secondaryPos;
    private Vector3 secondaryStartPos;
    public Transform SceneCamera; // takes the scene camera as its argument, allows us to pull local position and rotation of the camera to set ADS point

    // ADS variables
    public bool isADS = false; // controlling whether we are ADSed or not
    public bool fullADS; // signals when ADSing transition is fully complete or not, referenced in WeaponBob script
    private float transitionSpeed = 6f; // transition speed of interpolation

    // misc weapon vars, control whether we have our primary out or not
    private bool isPrimaryOut;
    private bool isSecondaryOut;
    private bool ADSready = true; // controls when we are able to ADS to prevent wacky ADSing in and out

    public AR_Shooting ar_ShootingPoint;
    public Pistol_Shooting pistol_ShootingPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        // getting rotation and position settings of both gun models on startup to later interpolate from and update
        primaryStartPos = primaryPos.localPosition;
        secondaryStartPos = secondaryPos.localPosition;

        Debug.Log("ADS Position in World Space: " + ADSpos);
    }

    // Update is called once per frame
    void Update() // ADS functionality
    {
        // ADS position calculation 
        Vector3 centerViewportPoint = new Vector3(0.5f, 0.5f, 0.8f); // AR ADSing (really any weapon with a scope on it)
        Vector3 farCenterViewportPoint = new Vector3(0.5f, 0.5f, 1.61f); // Iron sights ADSing
        
        ADSpos = Camera.main.ViewportToWorldPoint(centerViewportPoint);
        farADSpos = Camera.main.ViewportToWorldPoint(farCenterViewportPoint);
        
        ADSDelay(); // enables a delay between each attempted swap while ADSed, allows camera and weapon to properly swap without warping camera FOV, works in tandem with WeaponManager script

        // checking currently equiped weapon
        if (Input.GetKeyDown(KeyCode.Alpha1) && ADSready)
        {
            isPrimaryOut = true;
            isSecondaryOut = false;
            StartCoroutine(ADSDelay());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && ADSready)
        {
            isPrimaryOut = false;
            isSecondaryOut = true;
            StartCoroutine(ADSDelay());
        }
        
        // reading whether we are ADSed or not
        if (Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift))
        {
            isADS = true;
            fullADS = true;
            if (isPrimaryOut || isSecondaryOut)
            {
                crosshair.SetActive(false); // deactivates the crosshair onscreen if a weapon is out
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            isADS = false;
            crosshair.SetActive(true); // reactivate the crosshair onscreen
        }
        
        // reading whether we are ADSed or not
        if (isADS && isPrimaryOut && !Input.GetKey(KeyCode.LeftShift)) // not actively sprinting and trying to ADS
        {
            int targetFOV = 40; // FOV for ADSing in scope
            
            // adjust the field of view while ADSing
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * 7);

            // shift weapon over to center gun on ADS
            Transform shootingPoint = ar_ShootingPoint.ShootingPoint;
            
            // total distance from center of the screen to the dot sight/sights
            Vector3 directionVector = ADSpos - shootingPoint.position;
            
            // normalize the vector quantity
            Vector3 normalixedDir = directionVector.normalized;

            // calculated movement that we'll animate by for ADSing
            Vector3 movement = normalixedDir * transitionSpeed * Time.deltaTime;

            // ensure the movement magnitude is smaller than the original direction vector
            if (movement.magnitude < directionVector.magnitude)
            {
                primaryPos.position += movement;
            }
            else
            {
                // set the weapons' position exactly at the center of the screen with the dot being visible
                primaryPos.position = ADSpos - (shootingPoint.position - primaryPos.position);
            }
        }

        if (isADS && isSecondaryOut && !Input.GetKey(KeyCode.LeftShift))
        {
            int targetFOV = 40;

            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * 7);

            Transform shootingPoint = pistol_ShootingPoint.ShootingPoint;

            Vector3 directionVector = farADSpos - shootingPoint.position;
            
            Vector3 normalizedDirection = directionVector.normalized;
            Vector3 movement = normalizedDirection * transitionSpeed * Time.deltaTime;

            if (movement.magnitude < directionVector.magnitude)
            {
                secondaryPos.position += movement;
            }
            else
            {
                secondaryPos.position = farADSpos - (shootingPoint.position - secondaryPos.position);
            }
        }

        // if not ADSed/unADS
        else if (!isADS && isPrimaryOut || Input.GetKeyDown(KeyCode.Alpha2))
        { // same logic as ADSing in but now going backwards
            Vector3 directionVector = primaryStartPos - primaryPos.localPosition;
            Vector3 normalizedDirection = directionVector.normalized;

            Vector3 movement = normalizedDirection * transitionSpeed * Time.deltaTime;

            if (movement.magnitude < directionVector.magnitude)
            {
                primaryPos.position += movement;
            }
            else
            {
                primaryPos.localPosition = primaryStartPos;
            }

            // Adjust the field of view back to the default
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * transitionSpeed);
            fullADS = false;
        }

        else if (!isADS && isSecondaryOut || Input.GetKeyDown(KeyCode.Alpha1))
        {
            Vector3 directionVector = secondaryStartPos - secondaryPos.localPosition;
            Vector3 normalizedDirection = directionVector.normalized;

            Vector3 movement = normalizedDirection * transitionSpeed * Time.deltaTime;

            if (movement.magnitude < directionVector.magnitude)
            {
                secondaryPos.localPosition += movement;
            }
            else
            {
                secondaryPos.localPosition = secondaryStartPos;
            }

            // Adjust the field of view back to the default
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * transitionSpeed);
            fullADS = false;
        }
    }
    private IEnumerator ADSDelay() // effectively disables ADS functionality between weapon swaps, prevents "warping" of camera FOV
    {
        ADSready = false;
        yield return new WaitForSeconds(1f);
        ADSready = true;
    }
}