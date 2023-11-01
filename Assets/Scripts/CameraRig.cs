using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerRotate : MonoBehaviour
{
    public Transform playerCamera; // refers to the player camera, so we can controls its rotation and allow the player to look around
    public Transform gunModel; // refers to our gun model so we can anchor it properly to the crosshair
    public float rotateSpeed = 1.0f; // controls the speed at which the camera will rotate along an axis
    // Start is called before the first frame update
    void Start()
    { 
        gunModel.localEulerAngles = new Vector3(gunModel.localEulerAngles.x, 354f, gunModel.localEulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        // takes the user's horizontal and vertical mouse rotation to apply to the camera
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        playerCamera.Rotate(Vector3.up * mouseX * rotateSpeed); // rotates around the Y axis for horizontal rotation
        playerCamera.Rotate(Vector3.left * mouseY * rotateSpeed); // rotates around the X axis for vertical rotation

        playerCamera.localEulerAngles = new Vector3(playerCamera.localEulerAngles.x, playerCamera.localEulerAngles.y, 0);
        // this basically manipulates the angles so that the x and y axis are set to the camera'a values, however the z-axis is always set to 0, so we don't get trippy rotations while playing
    }
}
