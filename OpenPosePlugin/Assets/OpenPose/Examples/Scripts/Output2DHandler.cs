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
            //OPAPI.OPT_RegisterTest(FuncInt, FuncFloat, FuncByte);
            //OPAPI.OPT_CallbackTestFunctions();

            // Enable openpose log to unity
            OPAPI.OPEnableDebug(true);
            // Configure openpose with default value
            OPAPI.OPConfigure();
            // Start openpose
            OPAPI.OPRun();
        }

        /*private void FuncInt(ref IntPtr ptr, ref int size, int type){
            int[] pIntArray = new int[size];
            Marshal.Copy(ptr, pIntArray, 0, size);
            Debug.Log(pIntArray[3]);
        }

        private void FuncFloat(ref IntPtr ptr, ref int size, int type){
            float[] floatArr = new float[size];
            Marshal.Copy(ptr, floatArr, 0, size);
            Debug.Log(floatArr[2]);
            
        }

        private void FuncByte(ref IntPtr ptr, ref int size, int type){
            switch (type){
                case 0: 
                    //Debug.Log(ptr.ToPointer());
                    int[] pIntArray = new int[size];
                    Marshal.Copy(ptr, pIntArray, 0, size);
                    Debug.Log(pIntArray[3]);
                    break;
                case 1: 
                    //float[] floatArr = new float[size];
                    //Marshal.Copy(ptr, floatArr, 0, size);
                    //Debug.Log(floatArr[2]);
                    break;
            }
        }*/

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
