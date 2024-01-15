using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsCount; 

    private int LastFrameIndex;
    private float[] frameDeltaTimeArray;
    // Start is called before the first frame update
    void Awake()
    {
        frameDeltaTimeArray = new float[50]; // store deltatime readings, i.e. fps count
    }

    // Update is called once per frame
    void Update()
    { // replace fps times with new ones in set indices and update text based off FPS calculation below
		frameDeltaTimeArray[LastFrameIndex] = Time.deltaTime;
        LastFrameIndex = (LastFrameIndex + 1) % frameDeltaTimeArray.Length;

        fpsCount.text = "Client FPS: " + Mathf.RoundToInt(CalculateFPS()).ToString();
    }
    private float CalculateFPS()
    { // calculate average FPS from multiple frames
        float total = 0f;

        foreach (float deltaTime in frameDeltaTimeArray){
            total += deltaTime;
        }
        return frameDeltaTimeArray.Length / total;
    }
}
