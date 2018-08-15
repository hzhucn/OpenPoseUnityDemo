using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace op.examples
{
    public class OPWrapper : MonoBehaviour
    {
        [SerializeField] bool enableDebug = true;

        private void Awake()
        {
            OP_API.EnableDebug = enableDebug;
            OP_API.OPRegisterOutputCallback(OPOutput);
            OP_API.OPSetParameter(OPFlag.HAND);
            OP_API.OPRun();
        }

        private void OPOutput(string message, int type = 0)
        {
            Debug.Log("output");
            if (OutputController.instance != null)
            {
                switch (type)
                {
                    case 0:
                        //OutputController.instance.PushNewOutput(message);
                        //OutputController.instance.PushNewImage(imageData);
                        break;
                    case 1:
                        //OutputController.instance.PushNewImage(message);
                        break;
                }
            }
        }
        
    }
}
