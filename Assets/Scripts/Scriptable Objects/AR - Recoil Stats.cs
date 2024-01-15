using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // allows multiple instances of the scriptable obj to be used/created
public class ARRecoilStats : ScriptableObject
{
    public float recoilX = -1.8f;
    public float recoilY = 1.8f;
    public float recoilZ = 0.35f;

    public float ADS_recoilX = 1.25f;
    public float ADS_recoilY = 1;
    public float ADS_recoilZ = 0.25f;

    // Settings
    public float snappiness = 6; // controls recoil snappiness/speed 
    public float returnSpeed = 3.5f; // multiplier for returning weapon to OG rotation
}