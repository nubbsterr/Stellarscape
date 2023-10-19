using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShooting : MonoBehaviour
{
    public bool isFullAuto = false; // controls full auto firing and semiauto firing, depending on continous mouse input or not
    public float fireRate = 0.1f; // controls full auto firerate, the delay between shots in full auto
    public float bulletSpeed = 10.0f; // the speed at which bullets will travel
    public Transform gunBarrel; // point where bullets will exit the gun
    public GameObject bulletPrefab; // a variable that will take our AR_Bullet prefab as an argument, allowing us to clone the bullet for shooting

    private bool isShooting = false; // controls if a player is shooting or not
    private float maxRaycastDistance = 100.0f; // sets the effective distance of bullets, since targets can only be hit up to 100m (100.0f) by the raycast.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
            // Handle hit on the target (e.g., play a hit marker sound, provide visual feedback).
            // You can also handle target health and scoring here if needed.
            Debug.Log("Target Hit!");
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
        Gizmos.color = Color.red;
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