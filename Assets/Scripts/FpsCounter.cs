using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsCount; 
    public float UpdateFPS = 0.5f; // time between fps update on screen
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFPS -= Time.deltaTime;
        if (UpdateFPS <= 0f)
        {
            float fps = 1f / Time.unscaledDeltaTime; // amount of frames completed in one second
            fpsCount.text = "FPS: " + Mathf.Round(fps).ToString();
            UpdateFPS = 0.5f;
        }

    }
}
