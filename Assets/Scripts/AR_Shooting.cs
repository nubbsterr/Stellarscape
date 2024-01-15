using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.UI; // namespace for TextMeshPro to display ammo on-screen

public class AR_Shooting : MonoBehaviour
{

    // all public variables: (global booleans, variables, objects, etc.)
        public bool isFullAuto = false; // controls full auto firing and semiauto firing, depending on continous mouse input or not
        public bool isReloading = false; // controls when we are reloading or not
        public bool outofAmmo = false; // controls when we run out of ammo or not
        public float reloadTime = 2.0f; // 2 second delay between reloading, cannot shoot during this delay
        public float fireRate = 0.1f; // controls full auto firerate, the delay between shots in full auto
        public float bulletSpeed = 10.0f; // the speed at which bullets will travel
        public int currentMagAmmo = 30; // total mag ammo of the weapon, modifiable.
        public int totalAmmo = 120; // total amount of ammo that is currently being held
        
        public Transform gunBarrel; // point where bullets will exit the gun
        public Transform gunModel; // references our gun model in game (not really but we need this to update our gun barrel's position)
        public Transform ScopeBarrel; // references the sight of our AR model, allows for precision shooting while ADSed
        public Transform ejectionPort;
        public Transform ThreeD_model; // refers to our actual 3D model, forces rotation on startup to match crosshair
        [SerializeField] private Transform shootingPoint; // temp var for finding ADS point
        public Transform ShootingPoint
        {
            get { return shootingPoint; }
        }
        
        public GameObject bulletPrefab; // a variable that will take our AR_Bullet prefab as an argument, allowing us to clone the bullet for shooting
        public GameObject cartridge; // bullet casing model
        public GameObject crosshair; // refers to our crosshair on screen
        public GameObject hitmarker; // basic hitmarker image
        public GameObject killmarker; // shows upon killing an enemy
        public GameObject headshotmarker;
        public GameObject gunSound; // controls our gun SFX so it doesn't run on startup
        public GameObject reloadingSFX; // controls out reload SFX
       
        public TextMeshProUGUI currentMagText; // refers to our currentMagAmmo variable so we can "dynamically" display it on our HUD!
        public TextMeshProUGUI totalAmmoText; // same thing here but for our totalAmmo count!
        public TextMeshProUGUI fireSelection; // shifts between full-auto and semi-auto on screen, UI element
       
        public AudioClip gunSFX;
        public AudioClip hitSFX;
        public AudioClip killSFX;
        public AudioClip headshotSFX;
        public AudioClip reloadSFX;

        public ParticleSystem muzzleflash; // controls the appearance of our muzzle flash!
        private WeaponRecoil RecoilScript; // reference our Recoil function
    
    // private variables: (functions, private parameters, etc.)
        private bool isShooting = false; // controls if a player is shooting or not
        private bool isADSed;
        public int targetsHit = 0; // BURNER VARIABLE, DELETE ONCE DAMAGE DEALING AND PROPER DEATH IS SET
    
    // Start is called before the first frame update
    void Start()
    {
        RecoilScript = GameObject.Find("WeaponManager").GetComponent<WeaponRecoil>(); // acquire recoil script on startup
    }

    // Update is called once per frame 
    void Update()
    {
        BarrelPosUpdate(); // update gun barrel position
        Reloading(); // starts running our Reloading function

        // ADS control, disbale crosshair image when ADSed
        if (Input.GetMouseButtonDown(1))
        {
            isADSed = true;

            hitmarker.GetComponent<RawImage>().enabled = false;
            killmarker.GetComponent<RawImage>().enabled = false;
            headshotmarker.GetComponent<RawImage>().enabled = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isADSed = false;

            hitmarker.GetComponent<RawImage>().enabled = true;
            killmarker.GetComponent<RawImage>().enabled = true;
            headshotmarker.GetComponent<RawImage>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            isFullAuto = !isFullAuto; // Toggle between full-auto and semi-auto
            fireSelection.text = isFullAuto ? "Full-Auto" : "Semi-Auto"; // display onscreen current fire selector setting, dependant on player chosen setting
        }
        
        if (!outofAmmo && isFullAuto && Input.GetMouseButton(0)) // cannot shoot while sprinting!
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
        }
        else if (!outofAmmo && Input.GetMouseButtonDown(0)) // Semi-Auto firing mechanic
        {
            FireBullet();
        }
    }

    private void BarrelPosUpdate()
    {
        if (gunBarrel != null) // if there is a gunbarrel object stored in the variable slot
        {
            gunBarrel.position = gunModel.position; // update the gun barrel's position to the gun model's position and rotation
            gunBarrel.rotation = gunModel.rotation;
        }
    }

    private void Reloading() 
    {
        if (currentMagAmmo <= 10)
        {
            currentMagText.color = Color.red;
        }
        if (currentMagAmmo == 0)
        {
            outofAmmo = true; // cease all firing if empty, player has to reload
        }
        if(currentMagAmmo < 30 && (totalAmmo > 0 || currentMagAmmo == 0)) // if the player doesn't have a full mag or empty on ammo completely:
        {
            if (Input.GetKeyDown(KeyCode.R)) // reload key or if mag is empty (auto reload)
            {
                currentMagText.color = Color.red;
                isReloading = true;
                outofAmmo = true; // prevents shooting while reloading
                Debug.Log("Reloading!");
                if (totalAmmo > 30)
                {
                    int remainingAmmo = 30 - currentMagAmmo;
                    totalAmmo -= remainingAmmo; // subtract the remaining ammo from the total ammo
                    currentMagAmmo = 30; // refresh mag ammo
                    reloadingSFX.SetActive(true);
                    reloadingSFX.GetComponent<AudioSource>().PlayOneShot(reloadSFX);
                    StartCoroutine(ReloadDelay()); // starts reloading and engages reload timer, disables shooting capabilites
                }
                else 
                {
                    currentMagAmmo = totalAmmo;
                    totalAmmo = 0;
                    reloadingSFX.SetActive(true);
                    reloadingSFX.GetComponent<AudioSource>().PlayOneShot(reloadSFX);
                    StartCoroutine(ReloadDelay()); // starts reloading and engages reload timer, disables shooting capabilites
                }
            }
            else
            {
                if (currentMagAmmo == 0 && totalAmmo == 0)
                {
                    // if the player is totally out of ammo
                    currentMagText.color = Color.red;
                    totalAmmoText.color = Color.red;
                    outofAmmo = true; // prevent shooting while out of ammo
                }
                if (totalAmmo == 0)
                {
                    totalAmmoText.color = Color.red;
                }
            }
        }
    }

    private void FireBullet()
    {
        if (!outofAmmo)
        {
            // show muzzle flash
            muzzleflash.Play();

            // run recoil command
            RecoilScript.RecoilOnFire();

            // bullet ejection logic
            GameObject emptyCartridge = Instantiate(cartridge, ejectionPort.position, Quaternion.Euler(ejectionPort.localEulerAngles.x,180f,ejectionPort.localEulerAngles.z)); // massive line, this locks the rotation so the cartridge is facing forwards when we launch it out
            Rigidbody cartidge = emptyCartridge.GetComponent<Rigidbody>(); 
            
            if (Input.GetAxis("Horizontal") > 0)
            {
                cartidge.AddForce(ejectionPort.forward * 10f, ForceMode.Impulse);
            }
            if (Input.GetAxis("Horizontal") > 0 && Input.GetKey(KeyCode.LeftShift))
            {
                cartidge.AddForce(ejectionPort.forward * 30f, ForceMode.Impulse);
            }
            
            cartidge.AddForce(ejectionPort.forward * 11f, ForceMode.Impulse);
            Destroy(emptyCartridge, 3f);

            if (!isADSed)
            {
                bulletPrefab.GetComponent<TrailRenderer>().enabled = true;
                // Instantiate the bullet prefab at the gun barrel's position and rotation
                GameObject bulletInstance = Instantiate(bulletPrefab, gunBarrel.position, gunBarrel.rotation);

                // getting hitmarker assets and sending them to bulletCollision script for use!
                bulletInstance.GetComponent<bulletCollision>().hitmarker = hitmarker;
                bulletInstance.GetComponent<bulletCollision>().killmarker = killmarker;
                bulletInstance.GetComponent<bulletCollision>().headshotmarker = headshotmarker;
                
                gunSound.SetActive(true); // activates itself
                gunSound.GetComponent<AudioSource>().PlayOneShot(gunSFX); // plays gunSFX, temp audio, will change later down the line
            
                // Update UI Text and subtract ammo count
                currentMagAmmo = currentMagAmmo-1; // subtract 1 from the mag ammo
                currentMagText.text = currentMagAmmo.ToString();

                // Applies velocity to the bullet via RigidBody
                Rigidbody bulletRigidbody = bulletInstance.GetComponent<Rigidbody>();
                bulletRigidbody.velocity = gunBarrel.forward * bulletSpeed;

                Destroy(bulletInstance,5f); // destroy bullet after a short delay
            }
            if (isADSed)
            {
                bulletPrefab.GetComponent<TrailRenderer>().enabled = false; // disable trail renderer if ADSed
                // Instantiate the bullet prefab at the gun barrel's position and rotation
                GameObject bulletInstance = Instantiate(bulletPrefab, ScopeBarrel.position, ScopeBarrel.rotation);
                
                // getting hitmarker assets and sending them to bulletCollision script for use!
                bulletInstance.GetComponent<bulletCollision>().hitmarker = hitmarker;
                bulletInstance.GetComponent<bulletCollision>().killmarker = killmarker;
                bulletInstance.GetComponent<bulletCollision>().headshotmarker = headshotmarker;

                gunSound.SetActive(true); // activates itself
                gunSound.GetComponent<AudioSource>().PlayOneShot(gunSFX); // plays gunSFX, temp audio, will change later down the line
            
                // Update UI Text and subtract ammo count
                currentMagAmmo = currentMagAmmo-1; // subtract 1 from the mag ammo
                currentMagText.text = currentMagAmmo.ToString();

                // Applies velocity to the bullet via RigidBody
                Rigidbody bulletRigidbody = bulletInstance.GetComponent<Rigidbody>();
                bulletRigidbody.velocity = ScopeBarrel.forward * bulletSpeed;

                Destroy(bulletInstance,5f); // destroy bullet after a short delay
            }
        }
    }

    private IEnumerator FullAutoFire() // Coroutine for full-auto firing behavior (controls time between shots)
    {
        isShooting = true;
        while(!outofAmmo && isShooting && Input.GetMouseButton(0))
        {
            FireBullet();
            yield return new WaitForSeconds(fireRate); // this controls the time gap between shots (firerate), we can edit this as needed by changing the value of our fireRate float!
        }
        isShooting = false; // Set isShooting to false when the firing loop stops, ceases all shooting mechanics
    }

    private IEnumerator ReloadDelay() // controls the delay between shooting and reloading, temporarily disables shooting while reloading for a specified duration
    {
        yield return new WaitForSeconds(reloadTime);
        // reloading logic
        currentMagText.text = currentMagAmmo.ToString();
        totalAmmoText.text = totalAmmo.ToString(); // updates our totalAmmo count

        currentMagText.color = Color.white;
        totalAmmoText.color = Color.white;

        // reenabled shootlng and disables reloading
        outofAmmo = false;
        isReloading = false;
    }
}