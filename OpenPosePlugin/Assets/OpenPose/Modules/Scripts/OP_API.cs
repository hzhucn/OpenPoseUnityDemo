using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace op
{
    public delegate void OutputCallback(string output, int type);
    public delegate void DebugCallback(string message, int type);

    public static class OP_API
    {
        #region OP API
        [DllImport("openpose")] private static extern void OP_RegisterDebugCallback(DebugCallback callback);
        [DllImport("openpose")] private static extern void OP_SetDebugEnable(bool enable);
        [DllImport("openpose")] private static extern void OP_Run(bool enableOutput, OutputCallback callback);
        [DllImport("openpose")] private static extern void OP_Shutdown();

        [DllImport("openpose")] public static extern void OP_ConfigurePose(
            bool body_disable = false, 
            int net_resolution_x = -1, int net_resolution_y = 368, // Point
            int output_resolution_x = -1, int output_resolution_y = -1, // Point
            int keypoint_scale = 0, // ScaleMode
            int num_gpu = -1, int num_gpu_start = 0, int scale_number = 1, float scale_gap = 0.3f,
            int render_pose = -1, // bool _3d = false, int _3d_views = 1, bool flir_camera = false, // RenderMode
            string model_pose = "BODY_25", // PoseModel
            bool disable_blending = false, float alpha_pose = 0.6f, float alpha_heatmap = 0.7f,
            int part_to_show = 0, string model_folder = "models/", // moved
            bool heatmaps_add_parts = false, bool heatmaps_add_bkg = false, bool heatmaps_add_PAFs = false, // HeatMapType
            int heatmaps_scale = 2, // HeatMapScaleMode
            bool part_candidates = false, float render_threshold = 0.05f, int number_people_max = -1);

        [DllImport("openpose")] public static extern void OP_ConfigureHand(
            bool hand = false, 
            int hand_net_resolution_x = 368, int hand_net_resolution_y = 368, // Point
            int hand_scale_number = 1, float hand_scale_range = 0.4f, bool hand_tracking = false,
            int hand_render = -1, bool _3d = false, int _3d_views = 1, bool flir_camera = false, int render_pose = -1, // RenderMode
            float hand_alpha_pose = 0.6f, float hand_alpha_heatmap = 0.7f, float hand_render_threshold = 0.2f
        );
            
        [DllImport("openpose")] public static extern void OP_ConfigureFace(
            bool face = false, int face_net_resolution_x = 368, int face_net_resolution_y = 368,
            int face_renderer = -1, int render_pose = -1, 
            float face_alpha_pose = 0.6f, float face_alpha_heatmap = 0.7f, float face_render_threshold = 0.4f
        );
        [DllImport("openpose")] public static extern void OP_ConfigureExtra(
            bool _3d = false, int _3d_min_views = -1, bool _identification = false, int _tracking = -1,	int _ik_threads = 0
        );
        [DllImport("openpose")] public static extern void OP_ConfigureInput(
            int frame_first = 0, int frame_last = -1, // unsigned long long (uint64)
            bool process_real_time = false, bool frame_flip = false,
            int frame_rotate = 0, bool frames_repeat = false, 
            // Producer
            string image_dir = "", string video = "", string ip_camera = "", int camera = -1,
            bool flir_camera = false, int camera_resolution_x = -1, int camera_resolution_y = -1, double camera_fps = 30.0,
            string camera_parameter_folder = "models / cameraParameters / flir / ", bool frame_keep_distortion = false,
            int _3d_views = 1, int flir_camera_index = -1
        );
        [DllImport("openpose")] public static extern void OP_ConfigureOutput(            
            string write_keypoint = "",
            string write_keypoint_format = "yml", string write_json = "", string write_coco_json = "",
            string write_coco_foot_json = "", string write_images = "", string write_images_format = "png", string write_video = "",
            double camera_fps = 30.0, 
            string write_heatmaps = "", string write_heatmaps_format = "png", string write_video_adam = "",
            string write_bvh = "", string udp_host = "", string udp_port = "8051"
        );
        #endregion

        #region Settings & Control        
        // Parameters
        private static Dictionary<OPFlag, string> Parameters = new Dictionary<OPFlag, string>();

        // Thread 
        private static Thread opThread;
        #endregion

        public static void OPSetParameter(OPFlag param, string value = "")
        {
            if (param == OPFlag.NONE) return;
            if (Parameters.ContainsKey(param))
            {
                Parameters[param] = value;
            } else
            {
                Parameters.Add(param, value);
            }
        }

        public static void OPClearParameters()
        {
            Parameters.Clear();
        }

        public static void OPDebugEnable(bool enable = true){
            OP_SetDebugEnable(enable);
        }

        //public static void OPOutputEnable(bool enable = true){
        //    OP_SetOutputEnable(enable);
        //}

        //public static void OPRegisterOutputCallback(OutputCallback callback){
        //    OP_RegisterOutputCallback(callback);
        //}

        public static void OPRun(bool enableOutput, OutputCallback callback)
        {
            if (opThread != null && opThread.IsAlive)
            {
                Debug.Log("OP already started");
            } else
            {
                // Register OP log callback
                OP_RegisterDebugCallback(new DebugCallback(OPLog));
                // Start OP thread
                opThread = new Thread(new ThreadStart(OPExecuteThread));
                opThread.Start();
            }
        }

        public static void OPShutdown()
        {
            OP_Shutdown();
        }
        
        private static void OPLog(string message, int type = 0)
        {
            switch (type)
            {
                case 0: Debug.Log("OP_Log: " + message); break;
                case -1: Debug.LogError("OP_Error: " + message); break;
                case 1: Debug.LogWarning("OP_Warning: " + message); break;
            }
        }

        //protected virtual void OPOutput(string output, byte[] imageData, int type = 0) 
        //{
            //Debug.Log("ok here" + imageData[0].ToString());
            //Debug.Log("OP_Output: " + output);
        //}
        
        private static string[] GenerateArgs()
        {
            List<string> args = new List<string>();
            args.Add("program_name"); // First parameter always be the program name
            foreach (var p in Parameters)
            {
                args.Add(OPConfig.ParamInfo[p.Key].flagName);
                if (p.Value != "")
                {
                    args.Add(p.Value);
                }
            }
            return args.ToArray();
        }

        // OP thread
        private static void OPExecuteThread()
        {
            //Debug.Log("OP_Start");

            string[] args = GenerateArgs();
            try
            {
                //OP_SetParameters(args.Length, args);
            } catch(Exception err)
            {
                Debug.LogError("OP_ParamSettingError: " + err.Message);
            }

            try
            {
                //OP_Run(); 
            } catch(Exception err)
            {
                Debug.LogError("OP_RunError: " + err.Message);
                //OP_Shutdown();
            }
            
            //Debug.Log("OP_End");
        }
    }

    public enum OPFlag
    {
        NONE,
        LOGGING_LEVEL,
        DISABLE_MULTI_THREAD,
        PROFILE_SPEED,
        CAMERA,
        CAMERA_RESOLUTION,
        CAMERA_FPS,
        VIDEO,
        IMAGE_DIR,
        FLIR_CAMERA,
        FLIR_CAMERA_INDEX,
        IP_CAMERA,
        FRAME_FIRST,
        FRAME_LAST,
        FRAME_FLIP,
        FRAME_ROTATE,
        FRAMES_REPEAT,
        PROCESS_REAL_TIME,
        CAMERA_PARAMETER_FOLDER,
        FRAME_KEEP_DISTORTION,
        MODEL_FOLDER,
        OUTPUT_RESOLUTION,
        NUM_GPU,
        NUM_GPU_START,
        KEYPOINT_SCALE,
        NUMBER_PEOPLE_MAX,
        BODY_DISABLE,
        MODEL_POSE,
        NET_RESOLUTION,
        SCALE_NUMBER,
        SCALE_GAP,
        HEATMAPS_ADD_PARTS,
        HEATMAPS_ADD_BKG,
        HEATMAPS_ADD_PAFS,
        HEATMAPS_SCALE,
        PART_CANDIDATES,
        FACE,
        FACE_NET_RESOLUTION,
        HAND,
        HAND_NET_RESOLUTION,
        HAND_SCALE_NUMBER,
        HAND_SCALE_RANGE,
        HAND_TRACKING,
        _3D,
        _3D_MIN_VIEWS,
        _3D_VIEWS,
        IDENTIFICATION,
        TRACKING,
        IK_THREDS,
        PART_TO_SHOW,
        DISABLE_BLENDING,
        RENDER_THRESHOLD,
        RENDER_POSE,
        ALPHA_POSE,
        ALPHA_HEATMAP,
        FACE_RENDER_THRESHOLD,
        FACE_RENDER,
        FACE_ALPHA_POSE,
        FACE_ALPHA_HEATMAP,
        HAND_RENDER_THRESHOLD,
        HAND_RENDER,
        HAND_ALPHA_POSE,
        HAND_ALPHA_HEATMAP,
        FULLSCREEN,
        NO_GUI_VERBOSE,
        DISPLAY,
        WRITE_IMAGES,
        WRITE_IMAGES_FORMAT,
        WRITE_VIDEO,
        WRITE_JSON,
        WRITE_COCO_JSON,
        WRITE_COCO_FOOT_JSON,
        WRITE_HEATMAPS,
        WRITE_HEATMAPS_FORMAT,
        WRITE_KEYPOINT,
        WRITE_KEYPOINT_FORAMT
    }

    internal static class OPConfig
    {
        public static Dictionary<OPFlag, ParameterInfo> ParamInfo = SetParamInfo();
        private static Dictionary<OPFlag, ParameterInfo> SetParamInfo()
        {
            var tempParamInfo = new Dictionary<OPFlag, ParameterInfo>();

            #region Parameters
            // Debugging/Other
            tempParamInfo.Add(OPFlag.LOGGING_LEVEL, new ParameterInfo("--logging_level", "", "Debugging/Other"));
            tempParamInfo.Add(OPFlag.DISABLE_MULTI_THREAD, new ParameterInfo("--disable_multi_thread", "", "Debugging/Other"));
            tempParamInfo.Add(OPFlag.PROFILE_SPEED, new ParameterInfo("--profile_speed", "", "Debugging/Other"));

            // Producer
            tempParamInfo.Add(OPFlag.CAMERA, new ParameterInfo("--camera", "", "Producer"));
            tempParamInfo.Add(OPFlag.CAMERA_RESOLUTION, new ParameterInfo("--camera_resolution", "", "Producer"));
            tempParamInfo.Add(OPFlag.CAMERA_FPS, new ParameterInfo("--camera_fps", "", "Producer"));
            tempParamInfo.Add(OPFlag.VIDEO, new ParameterInfo("--video", "", "Producer"));
            tempParamInfo.Add(OPFlag.IMAGE_DIR, new ParameterInfo("--image_dir", "", "Producer"));
            tempParamInfo.Add(OPFlag.FLIR_CAMERA, new ParameterInfo("--flir_camera", "", "Producer"));
            tempParamInfo.Add(OPFlag.FLIR_CAMERA_INDEX, new ParameterInfo("--flir_camera_index", "", "Producer"));
            tempParamInfo.Add(OPFlag.IP_CAMERA, new ParameterInfo("--ip_camera", "", "Producer"));
            tempParamInfo.Add(OPFlag.FRAME_FIRST, new ParameterInfo("--frame_first", "", "Producer"));
            tempParamInfo.Add(OPFlag.FRAME_LAST, new ParameterInfo("--frame_last", "", "Producer"));
            tempParamInfo.Add(OPFlag.FRAME_FLIP, new ParameterInfo("--frame_flip", "", "Producer"));
            tempParamInfo.Add(OPFlag.FRAME_ROTATE, new ParameterInfo("--frame_rotate", "", "Producer"));
            tempParamInfo.Add(OPFlag.FRAMES_REPEAT, new ParameterInfo("--frames_repeat", "", "Producer"));
            tempParamInfo.Add(OPFlag.PROCESS_REAL_TIME, new ParameterInfo("--process_real_time", "", "Producer"));
            tempParamInfo.Add(OPFlag.CAMERA_PARAMETER_FOLDER, new ParameterInfo("--camera_parameter_folder", "", "Producer"));
            tempParamInfo.Add(OPFlag.FRAME_KEEP_DISTORTION, new ParameterInfo("--frame_keep_distortion", "", "Producer"));

            // OpenPose
            tempParamInfo.Add(OPFlag.MODEL_FOLDER, new ParameterInfo("--model_folder", "", "OpenPose"));
            tempParamInfo.Add(OPFlag.OUTPUT_RESOLUTION, new ParameterInfo("--output_resolution", "", "OpenPose"));
            tempParamInfo.Add(OPFlag.NUM_GPU, new ParameterInfo("--num_gpu", "", "OpenPose"));
            tempParamInfo.Add(OPFlag.NUM_GPU_START, new ParameterInfo("--num_gpu_start", "", "OpenPose"));
            tempParamInfo.Add(OPFlag.KEYPOINT_SCALE, new ParameterInfo("--keypoint_scale", "", "OpenPose"));
            tempParamInfo.Add(OPFlag.NUMBER_PEOPLE_MAX, new ParameterInfo("--number_people_max", "", "OpenPose"));

            // OpenPose Body Pose
            tempParamInfo.Add(OPFlag.BODY_DISABLE, new ParameterInfo("--body_disable", "", "OpenPose Body Pose"));
            tempParamInfo.Add(OPFlag.MODEL_POSE, new ParameterInfo("--model_pose", "", "OpenPose Body Pose"));
            tempParamInfo.Add(OPFlag.NET_RESOLUTION, new ParameterInfo("--net_resolution", "", "OpenPose Body Pose"));
            tempParamInfo.Add(OPFlag.SCALE_NUMBER, new ParameterInfo("--scale_number", "", "OpenPose Body Pose"));
            tempParamInfo.Add(OPFlag.SCALE_GAP, new ParameterInfo("--scale_gap", "", "OpenPose Body Pose"));

            // OpenPose Body Pose Heatmaps and Part Candidates
            tempParamInfo.Add(OPFlag.HEATMAPS_ADD_PARTS, new ParameterInfo("--heatmaps_add_parts", "", "OpenPose Body Pose Heatmaps and Part Candidates"));
            tempParamInfo.Add(OPFlag.HEATMAPS_ADD_BKG, new ParameterInfo("--heatmaps_add_bkg", "", "OpenPose Body Pose Heatmaps and Part Candidates"));
            tempParamInfo.Add(OPFlag.HEATMAPS_ADD_PAFS, new ParameterInfo("--heatmaps_add_PAFs", "", "OpenPose Body Pose Heatmaps and Part Candidates"));
            tempParamInfo.Add(OPFlag.HEATMAPS_SCALE, new ParameterInfo("--heatmaps_scale", "", "OpenPose Body Pose Heatmaps and Part Candidates"));
            tempParamInfo.Add(OPFlag.PART_CANDIDATES, new ParameterInfo("--part_candidates", "", "OpenPose Body Pose Heatmaps and Part Candidates"));

            // OpenPose Face
            tempParamInfo.Add(OPFlag.FACE, new ParameterInfo("--face", "", "OpenPose Face"));
            tempParamInfo.Add(OPFlag.FACE_NET_RESOLUTION, new ParameterInfo("--face_net_resolution", "", "OpenPose Face"));

            // OpenPose Hand
            tempParamInfo.Add(OPFlag.HAND, new ParameterInfo("--hand", "", "OpenPose Hand"));
            tempParamInfo.Add(OPFlag.HAND_NET_RESOLUTION, new ParameterInfo("--hand_net_resolution", "", "OpenPose Hand"));
            tempParamInfo.Add(OPFlag.HAND_SCALE_NUMBER, new ParameterInfo("--hand_scale_number", "", "OpenPose Hand"));
            tempParamInfo.Add(OPFlag.HAND_SCALE_RANGE, new ParameterInfo("--hand_scale_range", "", "OpenPose Hand"));
            tempParamInfo.Add(OPFlag.HAND_TRACKING, new ParameterInfo("--hand_tracking", "", "OpenPose Hand"));

            // OpenPose 3-D Reconstruction
            tempParamInfo.Add(OPFlag._3D, new ParameterInfo("--3d", "", "OpenPose 3-D Reconstruction"));
            tempParamInfo.Add(OPFlag._3D_MIN_VIEWS, new ParameterInfo("--3d_min_views", "", "OpenPose 3-D Reconstruction"));
            tempParamInfo.Add(OPFlag._3D_VIEWS, new ParameterInfo("--3d_views", "", "OpenPose 3-D Reconstruction"));

            // Extra algorithms
            tempParamInfo.Add(OPFlag.IDENTIFICATION, new ParameterInfo("--identification", "", "Extra algorithms"));
            tempParamInfo.Add(OPFlag.TRACKING, new ParameterInfo("--tracking", "", "Extra algorithms"));
            tempParamInfo.Add(OPFlag.IK_THREDS, new ParameterInfo("--ik_threds", "", "Extra algorithms"));

            // OpenPose Rendering
            tempParamInfo.Add(OPFlag.PART_TO_SHOW, new ParameterInfo("--part_to_show", "", "OpenPose Rendering"));
            tempParamInfo.Add(OPFlag.DISABLE_BLENDING, new ParameterInfo("--disable_blending", "", "OpenPose Rendering"));

            // OpenPose Rendering Pose
            tempParamInfo.Add(OPFlag.RENDER_THRESHOLD, new ParameterInfo("--render_threshold", "", "OpenPose Rendering Pose"));
            tempParamInfo.Add(OPFlag.RENDER_POSE, new ParameterInfo("--render_pose", "", "OpenPose Rendering Pose"));
            tempParamInfo.Add(OPFlag.ALPHA_POSE, new ParameterInfo("--alpha_pose", "", "OpenPose Rendering Pose"));
            tempParamInfo.Add(OPFlag.ALPHA_HEATMAP, new ParameterInfo("--alpha_heatmap", "", "OpenPose Rendering Pose"));

            // OpenPose Rendering Face
            tempParamInfo.Add(OPFlag.FACE_RENDER_THRESHOLD, new ParameterInfo("--face_render_threshold", "", "OpenPose Rendering Face"));
            tempParamInfo.Add(OPFlag.FACE_RENDER, new ParameterInfo("--face_render", "", "OpenPose Rendering Face"));
            tempParamInfo.Add(OPFlag.FACE_ALPHA_POSE, new ParameterInfo("--face_alpha_pose", "", "OpenPose Rendering Face"));
            tempParamInfo.Add(OPFlag.FACE_ALPHA_HEATMAP, new ParameterInfo("--face_alpha_heatmap", "", "OpenPose Rendering Face"));

            // OpenPose Rendering Hand
            tempParamInfo.Add(OPFlag.HAND_RENDER_THRESHOLD, new ParameterInfo("--hand_render_threshold", "", "OpenPose Rendering Hand"));
            tempParamInfo.Add(OPFlag.HAND_RENDER, new ParameterInfo("--hand_render", "", "OpenPose Rendering Hand"));
            tempParamInfo.Add(OPFlag.HAND_ALPHA_POSE, new ParameterInfo("--hand_alpha_pose", "", "OpenPose Rendering Hand"));
            tempParamInfo.Add(OPFlag.HAND_ALPHA_HEATMAP, new ParameterInfo("--hand_alpha_heatmap", "", "OpenPose Rendering Hand"));

            // Display
            tempParamInfo.Add(OPFlag.FULLSCREEN, new ParameterInfo("--fullscreen", "", "Display"));
            tempParamInfo.Add(OPFlag.NO_GUI_VERBOSE, new ParameterInfo("--no_gui_verbose", "", "Display"));
            tempParamInfo.Add(OPFlag.DISPLAY, new ParameterInfo("--display", "", "Display"));

            // Result Saving
            tempParamInfo.Add(OPFlag.WRITE_IMAGES, new ParameterInfo("--write_images", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_IMAGES_FORMAT, new ParameterInfo("--write_images_format", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_VIDEO, new ParameterInfo("--write_video", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_JSON, new ParameterInfo("--write_json", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_COCO_JSON, new ParameterInfo("--write_coco_json", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_COCO_FOOT_JSON, new ParameterInfo("--write_coco_foot_json", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_HEATMAPS, new ParameterInfo("--write_heatmaps", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_HEATMAPS_FORMAT, new ParameterInfo("--write_heatmaps_format", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_KEYPOINT, new ParameterInfo("--write_keypoint", "", "Result Saving"));
            tempParamInfo.Add(OPFlag.WRITE_KEYPOINT_FORAMT, new ParameterInfo("--write_keypoint_foramt", "", "Result Saving"));
            #endregion

            return tempParamInfo;
        }

        internal struct ParameterInfo
        {
            public string flagName { get; private set; }
            public string defaultValue { get; private set; }
            public string tag { get; private set; }

            public ParameterInfo(string flagName, string defaultValue = "", string tag = "")
            {
                this.flagName = flagName;
                this.defaultValue = defaultValue;
                this.tag = tag;
            }
        }
    }
}
