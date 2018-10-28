using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace OpenPose {
	public class OPWrapper : MonoBehaviour {

		# region Singleton instance
		private static OPWrapper _instance;
		public static OPWrapper instance {
			get {
				Debug.AssertFormat(_instance, "No OPWrapper instance found");
				return _instance;
			}
			private set {
				Debug.AssertFormat(!_instance, "Multiple OPWrapper instances found");
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
        public static void OPRun() {
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

        public static void OPShutdown() {
            OPAPI.OP_Shutdown();
        }
        
		// Log callback
        private static DebugCallback OPLog = delegate(string message, int type){
            switch (type){
                case 0: Debug.Log("OP_Log: " + message); break;
                case -1: Debug.LogError("OP_Error: " + message); break;
                case 1: Debug.LogWarning("OP_Warning: " + message); break;
            }
        };

		// Output callback
        private static OutputCallback OPOutput = delegate(IntPtr ptrPtr, int ptrSize, IntPtr sizePtr, int sizeSize, int outputType){
			// Safety check
            if (ptrSize <= 0 || sizeSize <= 0) return;

            // Parse ptrPtr to ptrArray
			var ptrArray = new IntPtr[ptrSize];
            Marshal.Copy(ptrPtr, ptrArray, 0, ptrSize);

            // Parse sizePtr to sizeArray
			var sizeArray = new int[sizeSize];
            Marshal.Copy(sizePtr, sizeArray, 0, sizeSize);

            // Write output to data struct
			OPOutputParser.ParseOutput(ref currentData, ptrArray, sizeArray, (OutputType)outputType);

            // Turn on the flag to suggest new output is received 
			dataFlag = true;
        };

        // OP thread
        private static void OPExecuteThread() {            
            // Register OP log callback
            OPAPI.OP_RegisterDebugCallback(new DebugCallback(OPLog));

            // Register OP output callback

            // Start OP with output callback
            OPAPI.OP_Run(outputEnabled, OPOutput);
        }

        private void OnDestroy() {
            // Stop openpose
            OPShutdown(); // TODO: investigate why this one crash
        }

		private void Update(){
			// If new data, clear flag after the frame
			if (dataFlag) StartCoroutine(ClearDataFlagCoroutine());

			// Shutdown
			if (Input.GetKeyDown(KeyCode.Escape)){
				OPShutdown();
			}
		}

		private IEnumerator ClearDataFlagCoroutine(){
			yield return new WaitForEndOfFrame();
			dataFlag = false;
			currentData = new OPDatum();
		}
	}
}