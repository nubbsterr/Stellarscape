using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Transform player; // refers to our player so we can rotate them
    public float rotateSpeed = 1.0f; // controls the speed at which the camera will rotate along an axis

    float xRotation;
    float yRotation;
    // Start is called before the first frame update
    void Start()
    { 
        Cursor.lockState = CursorLockMode.Locked; // cursor locked to the center of the screen and is invisible
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // takes the user's horizontal and vertical mouse rotation to apply to the camera
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * rotateSpeed;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * rotateSpeed;

        yRotation += mouseX;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // clamping so we can't rotate paste 90 degrees up or down
        
        // camera rotation
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0); // must be localRotation for recoil to work!!!
        // player rotation
        player.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
