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

        private int outputNum = 0;
        private float startTime = -1f;

        private void Awake()
        {
            OP_API.OP_Start();
            // Start OpenPose
            OP_API.OPRegisterOutputCallback(OPOutput);
            OP_API.OPOutputEnable(true);
            OP_API.OPDebugEnable(true);
            //OP_API.OPSetParameter(OPFlag.NUMBER_PEOPLE_MAX, "1");
            //OP_API.OPSetParameter(OPFlag.HAND);
            //OP_API.OPSetParameter(OPFlag.MODEL_FOLDER, Application.streamingAssetsPath + "/models");
            OP_API.OP_ConfigurePose(false, -1, 368, -1, -1, 0, -1, 0, 1, 0.3f, -1, "BODY_25", false, 0.6f, 0.7f, 0, 
                Application.streamingAssetsPath + "/models", false, false, false, 2, false, 0.05f, 1);
            OP_API.OP_ConfigureHand(true);
            OP_API.OP_ConfigureFace();
            OP_API.OP_ConfigureExtra();
            OP_API.OP_ConfigureInput();
            OP_API.OP_ConfigureOutput();
            OP_API.OPRun();
            //Debug.Log(OP_API.OP_TestFunction(true));
        }

        private void OnDestroy()
        {
            OP_API.OPShutdown();
        }

        private void OPOutput(string output, int type) // run in OpenPose thread
        {
            //Debug.Log("Output: " + output);
            if (type == 0)
            {
                if (outputFlag) Debug.Log("+1");
                outputFlag = true;
                tempOutput = output;
            }
            if (type == -1){
                Debug.Log(output);
            }
        }

        private void Update()
        {
            if (startTime < 0 && outputNum > 0){
                startTime = Time.time;
                StartCoroutine(LogFramerate());
            }
            if (outputFlag)
            {
                outputNum++;
                currentFrame = OPFrame.FromJson(tempOutput);
                if (currentFrame.units.Count > 0)
                {
                    human.PushNewUnitData(currentFrame.units[0]);
                    human.SetActive(true);
                }
                outputFlag = false;
            }
        }

        IEnumerator LogFramerate(){
            while (true){
                yield return new WaitForSeconds(2f);
                Debug.Log("Avg fr: " + outputNum / (Time.time - startTime));
            }
        }
    }
}
