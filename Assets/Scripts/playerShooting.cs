using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShooting : MonoBehaviour
{
    // all public variables: (global booleans, variables, objects, etc.)
        public bool isFullAuto = false; // controls full auto firing and semiauto firing, depending on continous mouse input or not
        public float fireRate = 0.1f; // controls full auto firerate, the delay between shots in full auto
        public float bulletSpeed = 10.0f; // the speed at which bullets will travel
        public Transform gunBarrel; // point where bullets will exit the gun
        public Transform gunModel; // references our gun model in game
        public GameObject bulletPrefab; // a variable that will take our AR_Bullet prefab as an argument, allowing us to clone the bullet for shooting
        public GameObject crosshair; // refers to our crosshair on screen
        public GameObject hitmarker; // basic hitmarker image
        public GameObject killmarker; // shows upon killing an enemy
    
    // private variables: (functions, private parameters, etc.)
        private bool isShooting = false; // controls if a player is shooting or not
        private bool isADS = false; // controls/checks if the player is ADS'ed or ADSing to display the crosshair or not
        private float maxRaycastDistance = 100.0f; // sets the effective distance of bullets, since targets can only be hit up to 100m (100.0f) by the raycast.
        private Transform GunBarrel; // used to update the gunbarrel's real position
        public int targetsHit = 0; // BURNER VARIABLE, DELETE ONCE DAMAGE DEALING AND PROPER DEATH IS SET
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ADS(); // starts running our ADS function

        if (Input.GetKeyDown(KeyCode.B))
        {
            isFullAuto = !isFullAuto; // Toggle between full-auto and semi-auto
            Debug.Log("Switched to " + (isFullAuto ? "Full Auto" : "Semi Auto"));
        }
        if (isFullAuto && Input.GetMouseButton(0))
        {
            if (!isShooting)
            {
                StartCoroutine(FullAutoFire()); 
                // A coroutine is a special type of method that can yield control back to Unity at specific points 
                // allowing you to execute code over multiple frames (in this case, our bullets being fired and shot between frames).
            }
        }
        else if (isShooting && (!isFullAuto || !Input.GetMouseButton(0))) // if we are shooting, and full auto is not triggered, and Mouse1 is released:
        {
            StopCoroutine(FullAutoFire());
            // In the case of FullAutoFire, it's used to simulate full-auto fire by repeatedly calling FireBullet at a specified rate.
            // The yield return new WaitForSeconds(fireRate) line controls the rate of fire. (found in Private EInumerator FullAutoFire function)
            isShooting = false;
            Debug.Log("mouse released check"); // rarely promnpts on full and semi-auto fire
        }
        else if (Input.GetMouseButtonDown(0)) // Semi-Auto firing mechanic
        {
            Debug.Log("Mouse input and firebullet check");
            FireBullet();
        }

    }

    private void ADS()
    {
        if (isADS == false)
        {
            crosshair.SetActive(true); // activates the crosshair onscreen
            if (Input.GetMouseButtonDown(1))
            {
                isADS = true;
                crosshair.SetActive(false); // deactivates the crosshair onscreen
            }
        }
        else if (isADS == true) // if we are ADSed
        {
            crosshair.SetActive(false); // crosshair is deactivated
            if (Input.GetMouseButtonUp(1)) // if mouse2 is released
            {
                isADS = false;
                crosshair.SetActive(true); // re-enables the crosshair
            }
        }
        if (gunBarrel != null) // if there is a gunbarrel object stored in the variable slot
        {
            gunBarrel.position = gunModel.position; // update the gun barrel's position to the gun model's position and rotation
            gunBarrel.rotation = gunModel.rotation;
        }
    }

    private void HandleHitMarker()
    {
    hitmarker.SetActive(true); // Display the hit marker

    // deactivates the hit marker after a specified delay
    Invoke("DeactivateHitMarker", 0.3f);
    }

    private void DeactivateHitMarker()
    {
        hitmarker.SetActive(false);
    }

    private void HandleKillMarker()
    {
    killmarker.SetActive(true); // Display the hit marker

    // deactivates the hit marker after a specified delay
    Invoke("DeactivateKillMarker", 1.0f);
    }

    private void DeactivateKillMarker()
    {
        killmarker.SetActive(false);
    }

    private void FireBullet()
    {
        Ray ray = new Ray(gunBarrel.position, gunBarrel.forward); 
        // Creates a new ray starting from the gun barrel's position and extending forward.
        // This ray represents the path the bullet will follow.
        RaycastHit hit; 
        // RaycastHit hit is a data structure that will store information about what the ray hits.
        // It's used to detect if the ray intersects with any objects and gather information about that intersection.

        if (Physics.Raycast(ray, out hit, maxRaycastDistance))
        { // these two condiitonals check if the raycast has hit anything with the tag Target, and if so, prints a debug log message to confirm.
            if (hit.collider.CompareTag("Target"))
            {
                targetsHit ++; // DELETE WITH REST OF VFX TESTING
                hitmarker.SetActive(true);
                hitmarker.GetComponent<AudioSource>().Play(); // plays hitmarker sound!
                HandleHitMarker();
                Debug.Log("Target Hit!");
            
                if (targetsHit >= 3) // CURRENTLY TESTING FOR KILL MARKER SOUND AND VFX, DELETE ONCE PROPER DEATH IS SET
                {
                    hitmarker.SetActive(false); // deactivates temporarily
                    HandleKillMarker();
                    killmarker.GetComponent<AudioSource>().Play(); // plays kill sound!!!
                    Debug.Log("Kill!");
                    targetsHit = 0;
                }
            }
        }

        // Instantiate the bullet prefab at the gun barrel's position and rotation
        GameObject bulletInstance = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);
        
        // Destroy the bullet clone after a delay (e.g., 10 seconds)
        Destroy(bulletInstance, 10.0f); // each bullet has its own delay between being destroyed, which can be modified here.

        // Applies velocity to the bullet via RigidBody
        Rigidbody bulletRigidbody = bulletInstance.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = gunBarrel.forward * bulletSpeed;

        Debug.DrawRay(gunBarrel.position, gunBarrel.forward * bulletSpeed * 10, Color.red, 3.0f);
    }
    private void OnDrawGizmos() // bullet tracer for debugging, will follow the bullet's path!
    {
        Gizmos.color = Color.yellow;
        Vector3 bulletStartPosition = gunBarrel.position;
        Vector3 bulletEndPosition = bulletStartPosition + gunBarrel.forward * bulletSpeed;
        Gizmos.DrawLine(bulletStartPosition, bulletEndPosition);
    }

    private IEnumerator FullAutoFire() // Coroutine for full-auto firing behavior (controls time between shots)
    {
        isShooting = true;
        while(isShooting && Input.GetMouseButton(0))
        {
            Debug.Log("enumerator check");
            FireBullet();
            yield return new WaitForSeconds(fireRate); // this controls the time gap between shots (firerate), we can edit this as needed by changing the value of our fireRate float!
        }
        isShooting = false; // Set isShooting to false when the firing loop stops, ceases all shooting mechanics
    }
}