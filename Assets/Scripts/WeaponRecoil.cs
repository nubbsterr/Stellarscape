using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    // ADS boolean acquiring
    [SerializeField] private ADS ADS_Script;
    
    // Bool
    private bool isAiming;

    // Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    // Hipfire recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float ADS_recoilX;
    [SerializeField] private float ADS_recoilY;
    [SerializeField] private float ADS_recoilZ;

    // Settings
    [SerializeField] private float snappiness; // controls recoil snappiness/speed 
    [SerializeField] private float returnSpeed; // multiplier for returning weapon to OG rotation
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isAiming = ADS_Script.isADS;

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime); // targetrotation is essentially the gun in hip fire with no recoil, i.e. no recoil, no rotation changes
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime); // currentrotation is the gun's active rotation, it will constantly try to reset the guns' rotation back to zero

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilOnFire()
    { // controls recoil between being ADSed and not (i.e. less recoil when ADSed vs hipfiring)
        if (isAiming)
        {
            targetRotation += new Vector3(ADS_recoilX, Random.Range(-ADS_recoilY, ADS_recoilY), Random.Range(-ADS_recoilZ, ADS_recoilZ));
        }
        else
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ)); 
            // procedural recoil, changes vertical rotation to go upwards, recoilY controls horizontal recoil; the gun will sway left/right randomly
        }
    }
}
