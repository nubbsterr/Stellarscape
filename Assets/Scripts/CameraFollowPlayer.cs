using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    // public variables
    public Transform Player; // refers to the player object, this is what the camera should follow
    public float followSpeed = 5.0f; // speed at which the camera will follow the player
    public Vector3 offset; // controls the distance between the player and the camera

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null) // if there is a player object
        {
            // Calculate the desired position for the camera.
            Vector3 desiredPosition = Player.position + offset; // camera distance
            
            // interpolates between the player and camera's position to follow the player
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }
}
