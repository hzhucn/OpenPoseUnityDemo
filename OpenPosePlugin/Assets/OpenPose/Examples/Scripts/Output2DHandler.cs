using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace op.examples
{
    public class Output2DHandler : MonoBehaviour
    {
        [SerializeField] HumanController2D human;

        private static bool outputFlag = false;
        private static string tempOutput;
        private OPFrame currentFrame;

        // Frame rate
        public float avgFrameRate = 0f;
        [Range(0f, 1f)] public float frameRateSmoothRatio = 0.8f;
        private float avgFrameTime = -1f;
        private float lastFrameTime = -1f;

        private void Awake()
        {
            // Start OpenPose
            OP_API.OPDebugEnable(true);

            OP_API.OP_ConfigurePose(false, -1, 368, -1, -1, 0, -1, 0, 1, 0.3f, -1, "BODY_25", false, 0.6f, 0.7f, 0, 
               Application.streamingAssetsPath + "/models", false, false, false, 2, false, 0.05f, 1);
            OP_API.OP_ConfigureHand(true);
            OP_API.OP_ConfigureFace();
            OP_API.OP_ConfigureExtra();
            //OP_API.OP_ConfigureInput(); // Dont Use This
            OP_API.OP_ConfigureOutput();

            OP_API.OPSetOutputCallback(true, OPOutput);
            OP_API.OPRun();
        }

        private void OnDestroy()
        {
            OP_API.OPShutdown();
        }

        // Called from OpenPose threads
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
                if (lastFrameTime < 0f) lastFrameTime = Time.time;
                else if (avgFrameTime < 0f) avgFrameTime = Time.time - lastFrameTime;
                else {
                    avgFrameTime = Mathf.Lerp(Time.time - lastFrameTime, avgFrameTime, frameRateSmoothRatio);
                    avgFrameRate = 1f / avgFrameTime;
                }
            }
        }
    }
}
