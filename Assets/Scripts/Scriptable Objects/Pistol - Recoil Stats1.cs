using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // allows multiple instances of the scriptable obj to be used/created
public class PistolRecoilStats : ScriptableObject
{
    public float recoilX = -1.4f;
    public float recoilY = 1.4f;
    public float recoilZ = 0.25f;

    public float ADS_recoilX = 1f;
    public float ADS_recoilY = 0.75f;
    public float ADS_recoilZ = 0.15f;

    // Settings
    public float snappiness = 6; // controls recoil snappiness/speed 
    public float returnSpeed = 5; // multiplier for returning weapon to OG rotation
}