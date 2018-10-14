using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace OpenPose {
    // Delegate for output callback
    //public delegate void OutputCallback(string output, int type);

    // Logging callback delegate
    public delegate void DebugCallback(string message, int type);

    // Output callback delegate
    public delegate void OutputCallback(ref IntPtr val, IntPtr sizes, int sizeSize, int valType, int outputType);
    //public delegate void OutputTestTest(ref IntPtr val, ref int size, int type);

    public static class OPAPI
    {
        #region OP API
        /*
        Send a callback function to openpose logging system. No message will be received if no callback is sent. 
        The function will be called in op::log() or op::logError().
         */ 
        //[DllImport("openpose")] public static extern void OPT_RegisterTest(OutputCallback<IntPtr> intFunc, OutputCallback<IntPtr> floatFunc, OutputTestTest byteFunc);
        //[DllImport("openpose")] public static extern void OPT_CallbackTestFunctions();

        [DllImport("openpose")] private static extern void OP_RegisterDebugCallback(DebugCallback callback);

        /*
        Enable / disable logging callback. Disable when you don't want to receive loggings from openpose. 
        Disable logging may increase the speed a little bit. 
         */
        [DllImport("openpose")] private static extern void OP_SetDebugEnable(bool enable);

        /*
        Run openpose and giving the output callback function. No output will be received if no callback is sent. 
        Disable if you don't want to receive any output
         */
        [DllImport("openpose")] private static extern void OP_Run(bool enableOutput, OutputCallback callback);

        /*
        Shut down openpose program if it is running. It may take several seconds to fully stop it. 
         */
        [DllImport("openpose")] private static extern void OP_Shutdown();

        /* Openpose configurations - please read openpose documentation to understand all the parameters*/
        [DllImport("openpose")] private static extern void OP_ConfigurePose(
            bool body_disable = false,  
            string model_folder = "models/", int number_people_max = -1, // moved
            int net_resolution_x = -1, int net_resolution_y = 368, // Point
            int output_resolution_x = -1, int output_resolution_y = -1, // Point
            int keypoint_scale = 0, // ScaleMode
            int num_gpu = -1, int num_gpu_start = 0, int scale_number = 1, float scale_gap = 0.3f,
            int render_pose = -1, // bool _3d = false, int _3d_views = 1, bool flir_camera = false, // RenderMode
            string model_pose = "BODY_25", // PoseModel
            bool disable_blending = false, float alpha_pose = 0.6f, float alpha_heatmap = 0.7f,
            int part_to_show = 0,
            bool heatmaps_add_parts = false, bool heatmaps_add_bkg = false, bool heatmaps_add_PAFs = false, // HeatMapType
            int heatmaps_scale = 2, // HeatMapScaleMode
            bool part_candidates = false, float render_threshold = 0.05f
        );
        [DllImport("openpose")] private static extern void OP_ConfigureHand(
            bool hand = false, 
            int hand_net_resolution_x = 368, int hand_net_resolution_y = 368, // Point
            int hand_scale_number = 1, float hand_scale_range = 0.4f, bool hand_tracking = false,
            int hand_render = -1, bool _3d = false, int _3d_views = 1, bool flir_camera = false, int render_pose = -1, // RenderMode
            float hand_alpha_pose = 0.6f, float hand_alpha_heatmap = 0.7f, float hand_render_threshold = 0.2f
        );
        [DllImport("openpose")] private static extern void OP_ConfigureFace(
            bool face = false, int face_net_resolution_x = 368, int face_net_resolution_y = 368,
            int face_renderer = -1, int render_pose = -1, 
            float face_alpha_pose = 0.6f, float face_alpha_heatmap = 0.7f, float face_render_threshold = 0.4f
        );
        [DllImport("openpose")] private static extern void OP_ConfigureExtra(
            bool _3d = false, int _3d_min_views = -1, bool _identification = false, int _tracking = -1,	int _ik_threads = 0
        );
        [DllImport("openpose")] private static extern void OP_ConfigureInput(
            int frame_first = 0, int frame_last = -1, // unsigned long long (uint64)
            bool process_real_time = false, bool frame_flip = false,
            int frame_rotate = 0, bool frames_repeat = false, 
            // Producer
            string image_dir = "", string video = "", string ip_camera = "", int camera = -1,
            bool flir_camera = false, int camera_resolution_x = -1, int camera_resolution_y = -1, double camera_fps = 30.0,
            string camera_parameter_folder = "models / cameraParameters / flir / ", bool frame_keep_distortion = false,
            int _3d_views = 1, int flir_camera_index = -1
        );
        [DllImport("openpose")] private static extern void OP_ConfigureOutput(            
            string write_keypoint = "",
            string write_keypoint_format = "yml", string write_json = "", string write_coco_json = "",
            string write_coco_foot_json = "", string write_images = "", string write_images_format = "png", string write_video = "",
            double camera_fps = 30.0, 
            string write_heatmaps = "", string write_heatmaps_format = "png", string write_video_adam = "",
            string write_bvh = "", string udp_host = "", string udp_port = "8051"
        );
        #endregion      

        // Thread 
        private static Thread opThread;

        // Output
        private static bool outputEnabled;
        private static OutputCallback outputCallback;

        // User functions
        public static void OPEnableDebug(bool enable = true){
            OP_SetDebugEnable(enable);
        }

        public static void OPConfigure(bool bodyEnabled = true, bool handEnabled = true, bool faceEnabled = false, int maxPeopleNum = -1){
            OP_ConfigurePose(!bodyEnabled, Application.streamingAssetsPath + "/models");
            OP_ConfigureHand(handEnabled);
            OP_ConfigureFace(faceEnabled);
            OP_ConfigureExtra();
            //OP_ConfigureInput(); // don't use this now
            OP_ConfigureOutput();
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
            OP_Shutdown();
        }
        
        private static DebugCallback OPLog = delegate(string message, int type){
            switch (type){
                case 0: Debug.Log("OP_Log: " + message); break;
                case -1: Debug.LogError("OP_Error: " + message); break;
                case 1: Debug.LogWarning("OP_Warning: " + message); break;
            }
        };

        private static OutputCallback OPOutput = delegate(ref IntPtr val, IntPtr sizes, int sizeSize, int valT, int outputT){
            ValType valType = (ValType) valT;
            OutputType outputType = (OutputType) outputT;
            
            int[] sizesArray = new int[sizeSize];
            Marshal.Copy(sizes, sizesArray, 0, sizeSize);
            int volume = 0;
            foreach(var i in sizesArray){ volume *= i; }

            switch (valType){
                case ValType.Byte: 
                    break;
                case ValType.Float:
                    break;
                case ValType.Int:
                    Debug.Log(pIntArray[3]);
                    break;
                case ValType.Long:
                    break;
                case ValType.String:
                    break;
                default: 
                    break;
            }
        };



        // OP thread
        private static void OPExecuteThread()
        {            
            // Register OP log callback
            OP_RegisterDebugCallback(new DebugCallback(OPLog));

            // Register OP output callback

            // Start OP with output callback
            OP_Run(outputEnabled, outputCallback);
        }
    }
}
