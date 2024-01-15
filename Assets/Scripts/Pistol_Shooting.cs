using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using UnityEngine.UI; // namespace for TextMeshPro to display ammo on-screen

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
        public Transform ejectionPort;
        [SerializeField] private Transform shootingPoint; // temp var for finding ADS point
        public Transform ShootingPoint
        {
            get { return shootingPoint; }
        }
        
        public GameObject bulletPrefab; // a variable that will take our AR_Bullet prefab as an argument, allowing us to clone the bullet for shooting
        public GameObject cartridge;
        public GameObject crosshair; // refers to our crosshair on screen
        public GameObject hitmarker; // basic hitmarker image
        public GameObject headshotmarker;
        public GameObject killmarker; // shows upon killing an enemy
        public GameObject gunSound; // controls our gun SFX so it doesn't run on startup
        
        public TextMeshProUGUI Pistol_currentMagText; // refers to our currentMagAmmo variable so we can "dynamically" display it on our HUD!
        public TextMeshProUGUI Pistol_totalAmmoText; // same thing here but for our totalAmmo count!
       
        public AudioClip gunSFX; // so we can play it but not clip/break it
        public AudioClip hitSFX;
        public AudioClip killSFX;
        public AudioClip headshotSFX;

        public ParticleSystem muzzleflash; // controls when muzzle flash is shown after firing
        private WeaponRecoil RecoilScript; // reference our Recoil function
    
    // private variables: (functions, private parameters, etc.)
        public int targetsHit = 0; // BURNER VARIABLE, DELETE ONCE DAMAGE DEALING AND PROPER DEATH IS SET
    
    // Start is called before the first frame update
    void Start() // cringe
    {
        RecoilScript = GameObject.Find("WeaponManager").GetComponent<WeaponRecoil>(); // acquire recoil script on startup
    }

    // Update is called once per frame
    void Update()
    {
        BarrelPosUpdate(); // update gunbarrel position to fire weapons properly
        Reloading(); // starts running our Reloading function

        // ADS control, disbale crosshair image when ADSed
        if (Input.GetMouseButtonDown(1))
        {
            hitmarker.GetComponent<RawImage>().enabled = false;
            killmarker.GetComponent<RawImage>().enabled = false;
            headshotmarker.GetComponent<RawImage>().enabled = false;
        }
        if (Input.GetMouseButtonUp(1))
        {
            hitmarker.GetComponent<RawImage>().enabled = true;
            killmarker.GetComponent<RawImage>().enabled = true;
            headshotmarker.GetComponent<RawImage>().enabled = true;
        }

        if (!outofAmmo && Input.GetMouseButtonDown(0)) // Semi-Auto firing mechanic, cannot fire while sprinting
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
        if (currentMagAmmo <= 3)
        {
            Pistol_currentMagText.color = Color.red;
        }
        if (currentMagAmmo == 0)
        {
            outofAmmo = true; // cease all firing if empty, player has to reload
        }
        if(currentMagAmmo < 17 && (totalAmmo > 0 || currentMagAmmo == 0)) // if the player doesn't have a full mag or empty on ammo completely:
        {
            if (Input.GetKeyDown(KeyCode.R)) // reload key or if mag is empty (auto reload)
            {
                Pistol_currentMagText.color = Color.red;
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
                    Pistol_currentMagText.color = Color.red;
                    Pistol_totalAmmoText.color = Color.red;
                    outofAmmo = true; // prevent shooting while out of ammo
                }
                if (totalAmmo == 0)
                {
                    Pistol_totalAmmoText.color = Color.red;
                }
            }
        }
    }

    private void FireBullet()
    {
        if (!outofAmmo)
        {
            // play muzzle flash effect
            muzzleflash.Play();

            // run recoil command
            RecoilScript.RecoilOnFire();

            // enable bullet tracers, in case they've been disabled by separate scripts
            bulletPrefab.GetComponent<TrailRenderer>().enabled = true;

            // bullet ejection logic
            GameObject emptyCartridge = Instantiate(cartridge, gunBarrel.position, Quaternion.Euler(ejectionPort.localEulerAngles.x,180f,ejectionPort.localEulerAngles.z)); // massive line, this locks the rotation so the cartridge is facing forwards when we launch it out
            Rigidbody cartidge = emptyCartridge.GetComponent<Rigidbody>(); 
            cartidge.AddForce(ejectionPort.forward * 4f, ForceMode.Impulse);
            
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
            Pistol_currentMagText.text = currentMagAmmo.ToString();

            // Applies velocity to the bullet via RigidBody
            Rigidbody bulletRigidbody = bulletInstance.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = gunBarrel.forward * bulletSpeed;

            Destroy(bulletInstance,5f); // destroy bullet after a short delay
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

        Pistol_currentMagText.color = Color.white;
        Pistol_totalAmmoText.color = Color.white;
        // reenabled shootlng and disables reloading
        outofAmmo = false;
        isReloading = false;
    }
}