using System;
using System.Runtime.InteropServices;

namespace OpenPose {
    // Delegate for output callback
    //public delegate void OutputCallback(string output, int type);

    // Logging callback delegate
    public delegate void DebugCallback(string message, int type);

    // Output callback delegate
    public delegate void OutputCallback(IntPtr ptrs, int ptrSize, IntPtr sizes, int sizeSize, int outputType);

    public static class OPAPI {
        /*
        Send a callback function to openpose logging system. No message will be received if no callback is sent. 
        The function will be called in op::log() or op::logError().
         */ 
        //[DllImport("openpose")] public static extern void OPT_RegisterTest(OutputCallback<IntPtr> intFunc, OutputCallback<IntPtr> floatFunc, OutputTestTest byteFunc);
        //[DllImport("openpose")] public static extern void OPT_CallbackTestFunctions();

        [DllImport("openpose")] public static extern void OP_RegisterDebugCallback(DebugCallback callback);

        /*
        Enable / disable logging callback. Disable when you don't want to receive loggings from openpose. 
        Disable logging may increase the speed a little bit. 
         */
        [DllImport("openpose")] public static extern void OP_SetDebugEnable(bool enable);

        /*
        Run openpose and giving the output callback function. No output will be received if no callback is sent. 
        Disable if you don't want to receive any output
         */
        [DllImport("openpose")] public static extern void OP_Run(bool enableOutput, OutputCallback callback);

        /*
        Shut down openpose program if it is running. It may take several seconds to fully stop it. 
         */
        [DllImport("openpose")] public static extern void OP_Shutdown();

        /* Openpose configurations - please read openpose documentation for explanation*/
        [DllImport("openpose")] public static extern void OP_ConfigurePose(
            bool body_disable = false,  
            string model_folder = "models/", int number_people_max = -1, // moved
            int net_resolution_x = -1, int net_resolution_y = 256, // Point
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
        [DllImport("openpose")] public static extern void OP_ConfigureHand(
            bool hand = false, 
            int hand_net_resolution_x = 320, int hand_net_resolution_y = 320, // Point
            int hand_scale_number = 1, float hand_scale_range = 0.4f, bool hand_tracking = false,
            int hand_render = -1, bool _3d = false, int _3d_views = 1, bool flir_camera = false, int render_pose = -1, // RenderMode
            float hand_alpha_pose = 0.6f, float hand_alpha_heatmap = 0.7f, float hand_render_threshold = 0.2f
        );
        [DllImport("openpose")] public static extern void OP_ConfigureFace(
            bool face = false, int face_net_resolution_x = 320, int face_net_resolution_y = 320,
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
    }
}
