using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwap : MonoBehaviour
{
    public GameObject PrimaryWeapon; // refers to our AR15 model, which we can use to swap guns with
    public GameObject SecondaryWeapon; // refers to our pistol mode for weapon swapping too
    public GameObject StowWeaponsSFX; // refers to our StowSFX audio clip 
    public GameObject PulloutWeaponSFX;
    
    private bool StowWeapon; // controls if weapons are drawn of stowed completely.
    private bool ReloadSwap_isReady; // this will control whether or not we can swap weapons. This is needed to fix the reload swapping bugs!
    private bool WeaponSwap_isReady; 
    
    private bool PrimaryOut; 
    private bool SecondaryOut; 


    public bool ActiveWeapon; // the weapon that is currently being held by the player
    public AudioClip StowSFX; // self explanatory lel
    public AudioClip PulloutSFX;

    
    // Start is called before the first frame update
    void Start()
    {
        StowWeapon = true;
        Debug.Log("Press 1 or 2 to swap between Primary and Secondary weapons.");
        ReloadSwap_isReady = true;
        WeaponSwap_isReady = true;

        PrimaryOut = false;
        SecondaryOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            ReloadSwap_isReady = false;
            StartCoroutine(DisableReloadSwap());
        }

        KeyCode PrimaryKey = KeyCode.Alpha1;
        KeyCode SecondaryKey = KeyCode.Alpha2;

        if (StowWeapon == true)
        {
            PrimaryWeapon.SetActive(false);
            SecondaryWeapon.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.V) && ReloadSwap_isReady && WeaponSwap_isReady && !StowWeapon) // weapons are ready to be stowed but not already stowed
        {
            StowWeaponsSFX.SetActive(true);
            StowWeaponsSFX.GetComponent<AudioSource>().PlayOneShot(StowSFX);

            PrimaryOut = false;
            SecondaryOut = false;
            
            StowWeapon = true; // disable/stows all weapons
        }


            if (Input.GetKeyDown(PrimaryKey) && ReloadSwap_isReady && WeaponSwap_isReady && !PrimaryOut) // pulls out primary weapon
            {
                PulloutWeaponSFX.SetActive(true);
                PulloutWeaponSFX.GetComponent<AudioSource>().PlayOneShot(PulloutSFX);
                PrimaryWeapon.SetActive(true);
                
                WeaponSwap_isReady = false;
                StartCoroutine(DisableWeaponSwap()); // employ swap delay to not be able to swap endlessly
                
                SecondaryWeapon.SetActive(false);
                
                PrimaryOut = true;
                SecondaryOut = false;

                ActiveWeapon = true;
                StowWeapon = false;
            }
            if (Input.GetKeyDown(SecondaryKey) && ReloadSwap_isReady && WeaponSwap_isReady && !SecondaryOut) // pulls out secondary weapon
            {
                PulloutWeaponSFX.SetActive(true);
                PulloutWeaponSFX.GetComponent<AudioSource>().PlayOneShot(PulloutSFX);
                SecondaryWeapon.SetActive(true);
                
                WeaponSwap_isReady = false;
                StartCoroutine(DisableWeaponSwap());
                
                PrimaryWeapon.SetActive(false);
                
                PrimaryOut = false;
                SecondaryOut = true;

                ActiveWeapon = false;
                StowWeapon = false;
            }
    }

    private IEnumerator DisableReloadSwap()
    {
        yield return new WaitForSeconds(2.3f); // global reload time and a little bit more, this allows guns to fully reload before being able to swap!
        ReloadSwap_isReady = true;
    }
    private IEnumerator DisableWeaponSwap()
    {
        yield return new WaitForSeconds(1f); // tiny delay between being able to swap weapons
        WeaponSwap_isReady = true;
    }
} 
