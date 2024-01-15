using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bulletCollision : MonoBehaviour
{

    public GameObject hitmarker;
    public GameObject killmarker;
    public GameObject headshotmarker;

    private int targetsHit;

    public AudioClip hitSFX;
    public AudioClip killSFX;
    public AudioClip headshotSFX;

    private Vector3 lastPosition; // hold last position of the bullet on startup and after every frame

    // Start is called before the first frame update
    void Start()
    { 
        // null chceks before checking collisions
        if (hitmarker == null || killmarker == null || headshotmarker == null)
        {
            Debug.LogError("One or more markers not found!");
        }
        if (hitSFX == null || killSFX == null || headshotSFX == null)
        {
            Debug.LogError("One or more SFX not found!");
        }
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 nextFrameDir = transform.position - lastPosition; // total distance from current pos to last frame pos, ray max distance

        RaycastHit rayhit;
        if (Physics.Raycast(transform.position, nextFrameDir, out rayhit, nextFrameDir.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            if (rayhit.collider.CompareTag("Head"))
            {
                Debug.LogWarning($"Raycast Hit: {rayhit.collider}");
            }
            if (rayhit.collider.CompareTag("Target"))
            {
                Debug.LogWarning($"Raycast Hit: {rayhit.collider}");
            }
        }
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleHitMarker()
    {
        hitmarker.SetActive(true); // Display the hit marker

        // deactivates the hit marker after a specified delay
        Invoke("DeactivateHitMarker", 0.2f);
    }

    private void DeactivateHitMarker()
    {
        hitmarker.SetActive(false);
    }

    private void HandleKillMarker()
    {
        killmarker.SetActive(true);
        Invoke("DeactivateKillMarker", 1.3f);
    }

    private void DeactivateKillMarker()
    {
        killmarker.SetActive(false);
    }

    private void HandleHeadShotMarker()
    {
        headshotmarker.SetActive(true);
        Invoke("DeactivateHeadShotMarker", 1.35f);
    }

    private void DeactivateHeadShotMarker()
    {
        headshotmarker.SetActive(false);
    }

    void OnTriggerEnter(Collider col) // main collision checker method
    {
        Debug.Log("collisions ran");
        if (col.gameObject.CompareTag("Target"))
        {
            Debug.Log("worked Target");
            targetsHit ++; // DELETE WITH REST OF VFX TESTING
            hitmarker.SetActive(true);
            hitmarker.GetComponent<AudioSource>().PlayOneShot(hitSFX); // plays hitmarker sound!
            HandleHitMarker();
            if (targetsHit >= 3) // CURRENTLY TESTING FOR KILL MARKER SOUND AND VFX, DELETE ONCE PROPER DEATH IS SET
            {
                hitmarker.SetActive(false); // deactivates temporarily
                HandleKillMarker();
                killmarker.GetComponent<AudioSource>().PlayOneShot(killSFX); // plays kill sound!!!
                Debug.Log("Kill!");
                targetsHit = 0;
            }
        }
        if (col.gameObject.CompareTag("Head")) // hit a headshot
        {
            Debug.Log("worked Head");
            hitmarker.SetActive(false);
            killmarker.SetActive(false);
            HandleHeadShotMarker();
            headshotmarker.GetComponent<AudioSource>().PlayOneShot(headshotSFX);
            targetsHit = 0;
        }
    }
}
