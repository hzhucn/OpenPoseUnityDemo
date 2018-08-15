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

        private void Awake()
        {
            // Start OpenPose
            OP_API.EnableDebug = true;
            OP_API.OPRegisterOutputCallback(OPOutput);
            OP_API.OPSetParameter(OPFlag.HAND);
            OP_API.OPRun();
        }

        private void OnDestroy() // For safety
        {
            OP_API.OPShutdown();
        }

        private void OPOutput(string output, int type) // run in OpenPose thread
        {
            //Debug.Log("Output: " + output);
            if (type == 0)
            {
                outputFlag = true;
                tempOutput = output;
            }
        }

        private void Update()
        {
            if (outputFlag)
            {
                currentFrame = OPFrame.FromJson(tempOutput);
                if (currentFrame.units.Count > 0)
                {
                    human.PushNewUnitData(currentFrame.units[0]);
                    human.SetActive(true);
                }
                outputFlag = false;
            }
        }
    }
}
