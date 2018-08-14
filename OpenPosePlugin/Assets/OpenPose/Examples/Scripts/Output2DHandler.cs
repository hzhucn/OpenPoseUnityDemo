using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace op.examples
{
    public class Output2DHandler : MonoBehaviour
    {
        [SerializeField] bool enableDebug = true;

        [SerializeField] HumanController2D human;

        private OPFrame currentFrame;

        private void Awake()
        {
            OP_API.EnableDebug = enableDebug;
            OP_API.OPRegisterOutputCallback(OPOutput);
            OP_API.OPSetParameter(OPFlag.HAND);
            OP_API.OPRun();
        }

        private void OPOutput(string output, byte[] imageData, int type) // run in OpenPose thread
        {
            Debug.Log("output");
            if (type == 0)
            {
                currentFrame = OPFrame.FromJson(output);
                human.PushNewUnitData(currentFrame.units[0]);
            }
        }
    }
}
