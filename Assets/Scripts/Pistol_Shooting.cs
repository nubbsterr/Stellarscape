using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations; // namespace for TextMeshPro to display ammo on-screen

public class Pistol_Shooting : MonoBehaviour
{

    // all public variables: (global booleans, variables, objects, etc.)
        public bool isReloading = false; // controls when we are reloading or not
        public bool outofAmmo = false; // controls when we run out of ammo or not
        public float reloadTime = 2.0f; // 2 second delay between reloading, cannot shoot during this delay
        public float bulletSpeed = 10.0f; // the speed at which bullets will travel
        public int currentMagAmmo = 17; // total mag ammo of the weapon, modifiable.
        public int totalAmmo = 51; // total amount of ammo that is currently being held
        public Transform gunBarrel; // point where bullets will exit the gun
        public Transform gunModel; // references our gun model in game
        public GameObject bulletPrefab; // a variable that will take our AR_Bullet prefab as an argument, allowing us to clone the bullet for shooting
        public GameObject crosshair; // refers to our crosshair on screen
        public GameObject hitmarker; // basic hitmarker image
        public GameObject gunSound; // controls our gun SFX so it doesn't run on startup
        public GameObject killmarker; // shows upon killing an enemy
        public TextMeshProUGUI Pistol_currentMagText; // refers to our currentMagAmmo variable so we can "dynamically" display it on our HUD!
        public TextMeshProUGUI Pistol_totalAmmoText; // same thing here but for our totalAmmo count!
        public AudioClip gunSFX; // so we can play it but not clip/break it
        public AudioClip hitSFX;
        public AudioClip killSFX;
    
    // private variables: (functions, private parameters, etc.)
        private bool isADS = false; // controls/checks if the player is ADS'ed or ADSing to display the crosshair or not
        private float maxRaycastDistance = 80.0f; // sets the effective distance of bullets, since targets can only be hit up to 100m (100.0f) by the raycast.
        public int targetsHit = 0; // BURNER VARIABLE, DELETE ONCE DAMAGE DEALING AND PROPER DEATH IS SET
    
    // Start is called before the first frame update
    void Start() // cringe
    {

    }

    // Update is called once per frame
    void Update()
    {
        ADS(); // starts running our ADS function
        Reloading(); // starts running our Reloading function

        if (!outofAmmo && Input.GetMouseButtonDown(0)) // Semi-Auto firing mechanic
        {
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

    private void Reloading() 
    {
        if (currentMagAmmo == 0)
        {
            outofAmmo = true; // cease all firing if empty, player has to reload
        }
        if(currentMagAmmo < 17 && (totalAmmo > 0 || currentMagAmmo == 0)) // if the player doesn't have a full mag or empty on ammo completely:
        {
            if (Input.GetKeyDown(KeyCode.R)) // reload key or if mag is empty (auto reload)
            {
                isReloading = true;
                outofAmmo = true; // prevents shooting while reloading
                Debug.Log("Reloading!");
                if (totalAmmo > 17)
                {
                    int remainingAmmo = 17 - currentMagAmmo;
                    totalAmmo -= remainingAmmo; // subtract the remaining ammo from the total ammo
                    currentMagAmmo = 17; // refresh mag ammo
                    StartCoroutine(ReloadDelay()); // starts reloading and engages reload timer, disables shooting capabilites
                }
                else 
                {
                    currentMagAmmo = totalAmmo;
                    totalAmmo = 0;
                    StartCoroutine(ReloadDelay()); // starts reloading and engages reload timer, disables shooting capabilites
                }
            }
             else
            {
                if (currentMagAmmo == 0 && totalAmmo == 0)
                {
                    // if the player is totally out of ammo
                    Debug.Log("Out of Ammo!");
                    outofAmmo = true; // prevent shooting while out of ammo
                }
            }
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

    private void HandleGunSFX()
    {
        gunSound.SetActive(true); // Display the hit marker

        // deactivates the hit marker after a specified delay
        Invoke("DeactivateGunSFX", 0.6f);
    }
    private void DeactivateGunSFX() // enables and reenables gun sfx after a bullet is fired
    {
        gunSound.SetActive(false); 
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
                hitmarker.GetComponent<AudioSource>().PlayOneShot(hitSFX); // plays hitmarker sound!
                HandleHitMarker();
                Debug.Log("Target Hit!");
            
                if (targetsHit >= 4) // CURRENTLY TESTING FOR KILL MARKER SOUND AND VFX, DELETE ONCE PROPER DEATH IS SET
                {
                    hitmarker.SetActive(false); // deactivates temporarily
                    HandleKillMarker();
                    killmarker.GetComponent<AudioSource>().PlayOneShot(killSFX); // plays kill sound!!!
                    Debug.Log("Kill!");
                    targetsHit = 0;
                }
            }
        }
        if (!outofAmmo)
        {
            // Instantiate the bullet prefab at the gun barrel's position and rotation
            GameObject bulletInstance = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);
            
            gunSound.SetActive(true); // activates itself
            gunSound.GetComponent<AudioSource>().PlayOneShot(gunSFX); // plays gunSFX, temp audio, will change later down the line
            HandleGunSFX();
        
            currentMagAmmo = currentMagAmmo-1; // subtract 1 from the mag ammo
            Debug.Log(currentMagAmmo); // print the current mag ammo
        
            // Update the UI text for currentMagAmmo
            Pistol_currentMagText.text = currentMagAmmo.ToString();
        
            // Destroy the bullet clone after a delay (e.g., 10 seconds)
            Destroy(bulletInstance, 10.0f); // each bullet has its own delay between being destroyed, which can be modified here.

            // Applies velocity to the bullet via RigidBody
            Rigidbody bulletRigidbody = bulletInstance.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = gunBarrel.forward * bulletSpeed;

            Debug.DrawRay(gunBarrel.position, gunBarrel.forward * bulletSpeed * 10, Color.red, 3.0f);
        }
    }
    private void OnDrawGizmos() // displays the gun barrel's direction at which bullets will fire at (fake news, don't trust this unless in game runtime)
    {
        Gizmos.color = Color.blue;
        Vector3 bulletStartPosition = gunBarrel.position;
        Vector3 bulletEndPosition = bulletStartPosition + gunBarrel.forward * bulletSpeed;
        Gizmos.DrawLine(bulletStartPosition, bulletEndPosition);
    }

    private IEnumerator ReloadDelay() // controls the delay between shooting and reloading, temporarily disables shooting while reloading for a specified duration
    {
        yield return new WaitForSeconds(reloadTime);
        // reloading logic
        Pistol_currentMagText.text = currentMagAmmo.ToString();
        Pistol_totalAmmoText.text = totalAmmo.ToString(); // updates our totalAmmo count

        // reenabled shootlng and disables reloading
        outofAmmo = false;
        isReloading = false;
    }
}