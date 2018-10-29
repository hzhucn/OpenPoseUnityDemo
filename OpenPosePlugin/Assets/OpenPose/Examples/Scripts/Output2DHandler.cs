using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.InteropServices;

namespace OpenPose.Example
{
    public class Output2DHandler : MonoBehaviour
    {
        // The 2D human to control
        [SerializeField] HumanController2D human;

        // Output control
        private OPDatum datum;

        // Frame rate calculation
        public float avgFrameRate = 0f;
        [Range(0f, 1f)] 
        public float frameRateSmoothRatio = 0.8f;
        private float avgFrameTime = -1f;
        private float lastFrameTime = -1f;

        private void Start() {
            // Enable openpose log to unity
            OPWrapper.OPEnableDebug(true);
            // Enable openpose output to unity
            OPWrapper.OPEnableOutput(true);
            // Configure openpose with default value
            OPWrapper.OPConfigure(true, true, true, 1);
            // Start openpose
            OPWrapper.OPRun();
        }

        private void Update() {
            // New data received
            if (OPWrapper.OPGetOutput(out datum)){
                
                // Draw the first person in data
                human.DrawHuman(ref datum, 0);

                // Calculate framerate
                if (lastFrameTime > 0f) {
                    if (avgFrameTime < 0f) avgFrameTime = Time.time - lastFrameTime;
                    else {
                        avgFrameTime = Mathf.Lerp(Time.time - lastFrameTime, avgFrameTime, frameRateSmoothRatio);
                        avgFrameRate = 1f / avgFrameTime;
                    }
                }
                lastFrameTime = Time.time;
            }
        }

        private void OnGUI(){
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height),"Avg Frame Rate: " + avgFrameRate);
        }
    }
}
