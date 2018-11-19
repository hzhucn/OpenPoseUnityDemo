using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.InteropServices;

namespace OpenPose.Example {
    public class Output2DHandler : MonoBehaviour {

        // The 2D human to control
        [SerializeField] List<HumanController2D> humans;
        [SerializeField] ImageRenderer imageRenderer;

        // Output control
        private OPDatum datum;

        // Bg image
        private bool renderBgImg = false;
        public void ToggleRenderBgImg(){
            renderBgImg = !renderBgImg;
            OPWrapper.OPEnableImageOutput(renderBgImg);
            imageRenderer.FadeInOut(renderBgImg);
        }

        // Frame rate calculation
        [Range(0f, 1f)] 
        public float frameRateSmoothRatio = 0.8f;
        private float avgFrameRate = 0f;
        private float avgFrameTime = -1f;
        private float lastFrameTime = -1f;

        private void Start() {
            // Configure openpose with default value
            OPWrapper.OPConfigureAllInDefault();
            // Enable openpose log to unity
            OPWrapper.OPEnableDebug(true);
            // Enable openpose output to unity
            OPWrapper.OPEnableOutput(true);
            // Start openpose
            OPWrapper.OPRun();
        }

        private void Update() {
            // New data received
            if (OPWrapper.OPGetOutput(out datum)){
                
                // Draw human in data
                for (int i = 0; i < humans.Count; i++){
                    humans[i].DrawHuman(ref datum, i);
                }

                // Draw image
                imageRenderer.UpdateImage(ref datum);

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
