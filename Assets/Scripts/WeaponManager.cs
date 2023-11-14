using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject PrimaryWeapon; // refers to our AR15 model, which we can use to swap guns with
    public GameObject SecondaryWeapon; // refers to our pistol mode for weapon swapping too
    private bool StowWeapon; // controls if weapons are drawn of stowed completely.
    public bool ActiveWeapon; // the weapon that is currently being held by the player
    // Start is called before the first frame update
    void Start()
    {
        StowWeapon = true;
        Debug.Log("Press 1 or 2 to swap between Primary and Secondary weapons.");
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode PrimaryKey = KeyCode.Alpha1;
        KeyCode SecondaryKey = KeyCode.Alpha2;

        if (StowWeapon == true)
        {
            PrimaryWeapon.SetActive(false);
            SecondaryWeapon.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            StowWeapon = true; // disable/stows all weapons
            Debug.Log("Weapons stowed.");
        }


            if (Input.GetKeyDown(PrimaryKey)) // pulls out primary weapon
            {
                PrimaryWeapon.SetActive(true);
                SecondaryWeapon.SetActive(false);
                ActiveWeapon = true;
                StowWeapon = false;
            }
            if (Input.GetKeyDown(SecondaryKey)) // pulls out secondary weapon
            {
                SecondaryWeapon.SetActive(true);
                PrimaryWeapon.SetActive(false);
                ActiveWeapon = false;
                StowWeapon = false;
            }
        
        if (Input.GetKeyDown(KeyCode.Q) && StowWeapon == false) // if they have a weapon out and nothing is stowed, they also must hit Q to show this status
        {
            Debug.Log("Currently holding " + (ActiveWeapon ? "Primary" : "Secondary") + " Weapon"); // displays to the user what weapon they're holding out
            // if true, they're holding out their primary, if false, secondary.
        }
    }
} 
