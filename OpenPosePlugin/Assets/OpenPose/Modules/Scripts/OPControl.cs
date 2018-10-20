using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace OpenPose {
	public class OPControl : MonoBehaviour {

		# region Singleton instance
		private static OPControl _instance;
		public static OPControl instance {
			get {
				Debug.AssertFormat(_instance, "No OPControl instance found");
				return _instance;
			}
			private set {
				Debug.AssertFormat(!_instance, "Multiple OPControl instances found");
				_instance = value;
			}
		}
		private void Awake(){ instance = this; }
		#endregion

		// Output
        private static OPDatum currentData;
        private static bool outputEnabled = true;
		private static bool dataFlag = false;

        // Thread 
        private static Thread opThread;

        // User functions
        public static bool OPGetOutput(out OPDatum data){
            data = currentData;
			return dataFlag;
        }

        public static void OPEnableDebug(bool enable = true){
            OPAPI.OP_SetDebugEnable(enable);
        }

        public static void OPConfigure(bool bodyEnabled = true, bool handEnabled = true, bool faceEnabled = false, int maxPeopleNum = -1){
            OPAPI.OP_ConfigurePose(!bodyEnabled, Application.streamingAssetsPath + "/models");
            OPAPI.OP_ConfigureHand(handEnabled);
            OPAPI.OP_ConfigureFace(faceEnabled);
            OPAPI.OP_ConfigureExtra();
            //OPAPI.OP_ConfigureInput(); // don't use this now
            OPAPI.OP_ConfigureOutput();
        }

        public static void OPRun()
        {
            if (opThread != null && opThread.IsAlive)
            {
                Debug.Log("OP already started");
            } else
            {
                // Start OP thread
                opThread = new Thread(new ThreadStart(OPExecuteThread));
                opThread.Start();
            }
        }

        public static void OPShutdown()
        {
            OPAPI.OP_Shutdown();
        }
        
        private static DebugCallback OPLog = delegate(string message, int type){
            switch (type){
                case 0: Debug.Log("OP_Log: " + message); break;
                case -1: Debug.LogError("OP_Error: " + message); break;
                case 1: Debug.LogWarning("OP_Warning: " + message); break;
            }
        };

        private static OutputCallback OPOutput = delegate(ref IntPtr valPtr, IntPtr sizePtr, int sizeSize, int valType, int outputType){
			var sizeArray = new int[sizeSize];
            Marshal.Copy(sizePtr, sizeArray, 0, sizeSize);
			ParseOutput(valPtr, sizeArray, (OutputType)outputType);
			dataFlag = true;
        };

        private static void ParseOutput(IntPtr valPtr, int[] sizeArray, OutputType type){        
            int volume = 1;
            foreach(var i in sizeArray){ volume *= i; }

			switch (type){
				case OutputType.PoseKeypoints: {
					var valArray = new float[volume];
					Marshal.Copy(valPtr, valArray, 0, volume);
					currentData.poseKeypoints = new MultiArray<float>(valArray, sizeArray);
					break;
				}
				case OutputType.HandKeypoints: {
					Debug.Log("handhand");
					var valArray = new float[volume];
					Marshal.Copy(valPtr, valArray, 0, volume);
					int[] newSizeArray = sizeArray.Skip(1).ToArray();
					var handKeypointsL = new MultiArray<float>(valArray, newSizeArray);// Working on
					//var handKeypointsR = new MultiArray<float>(valArray2, newSizeArray);
					//currentData.handKeypoints = new Pair<MultiArray<float>>(handKeypointsL, handKeypointsR);
					break;
				}
			}
        }

        // OP thread
        private static void OPExecuteThread() {            
            // Register OP log callback
            OPAPI.OP_RegisterDebugCallback(new DebugCallback(OPLog));

            // Register OP output callback

            // Start OP with output callback
            OPAPI.OP_Run(outputEnabled, OPOutput);
        }

        private void OnDestroy()
        {
            // Stop openpose
            //OPShutdown(); // TODO: investigate why this one crash
        }

		private void Update(){
			if (dataFlag) StartCoroutine(ClearDataFlagCoroutine());
			if (Input.GetKeyDown(KeyCode.Escape)){
				OPShutdown();
			}
		}

		private IEnumerator ClearDataFlagCoroutine(){
			yield return new WaitForEndOfFrame();
			dataFlag = false;
		}
	}
}