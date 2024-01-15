using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLean : MonoBehaviour
{
    Quaternion startRotation;
    public float leanDegrees, slerpMultipler; 
    // degree measurement for leaning (i.e. degree turn on leaning that affects the camera, weapons, etc)
    // slerpMultipler controls how fast leaning will happen

    void Start()
    {
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q)) // left lean
        {
            Quaternion leanRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + leanDegrees);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, leanRotation, Time.deltaTime * slerpMultipler);
        }
        if (Input.GetKey(KeyCode.E)) // right lean
        {
            Quaternion leanRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z - leanDegrees);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, leanRotation, Time.deltaTime * slerpMultipler);
        }
        else // not leaning anymore or just sitting still
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, Time.deltaTime * slerpMultipler);
        }
    }
}
