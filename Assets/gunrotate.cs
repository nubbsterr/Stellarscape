using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunrotate : MonoBehaviour
{
    public Transform playerCamera; // references the player object
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = playerCamera.rotation;
        transform.position = playerCamera.position;
    }
}
