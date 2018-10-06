using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenPose.Example
{
    public class Output2DHandler : MonoBehaviour
    {
        // The 2D human to control
        [SerializeField] HumanController2D human;

        // Output control
        private static bool outputFlag = false;
        private static string tempOutput;
        private OPFrame currentFrame;

        // Frame rate calculation
        public float avgFrameRate = 0f;
        [Range(0f, 1f)] public float frameRateSmoothRatio = 0.8f;
        private float avgFrameTime = -1f;
        private float lastFrameTime = -1f;

        private void Awake()
        {
            // Enable openpose log to unity
            OPAPI.OPEnableDebug(true);
            // Configure openpose with default value
            OPAPI.OPConfigure();
            // Enable receiving output and send the callback function
            OPAPI.OPSetOutputCallback(true, OPOutput);
            // Start openpose
            OPAPI.OPRun();
        }

        private void OnDestroy()
        {
            // Stop openpose
            OPAPI.OPShutdown();
        }

        // Output callback from openpose
        // Note: it is good to make this function as simple as possible, to reduce the workload of openpose. 
        private void OPOutput(string output, int type)
        {
            if (type == 0) // Normal output
            {
                // Turn on flag and store the data
                outputFlag = true;
                tempOutput = output;
            }
        }

        private void Update()
        {
            // When output received
            if (outputFlag)
            {
                // Parse data
                currentFrame = OPFrame.FromJson(tempOutput);

                // Draw the first person in data
                if (currentFrame.units.Count > 0)
                {
                    human.PushNewUnitData(currentFrame.units[0]);
                    human.SetActive(true);
                }

                // Turn off output flag to receive next data
                outputFlag = false;

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
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height),"AVG Frame Rate: " + avgFrameRate);
        }
    }
}
