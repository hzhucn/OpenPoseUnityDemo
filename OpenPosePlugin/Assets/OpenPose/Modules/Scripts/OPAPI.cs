using System;
using System.Runtime.InteropServices;

namespace OpenPose {
    // Delegate for output callback
    //public delegate void OutputCallback(string output, int type);

    // Logging callback delegate
    public delegate void DebugCallback(string message, int type);

    // Output callback delegate
    public delegate void OutputCallback(IntPtr ptrs, int ptrSize, IntPtr sizes, int sizeSize, byte outputType);

    public static class OPAPI {
        /*
         * Send a callback function to openpose output. No output will be received if no callback is sent. 
         * Enable/disable the output callback. Can be set at runtime.
         */ 
        [DllImport("openpose")] public static extern void OP_RegisterOutputCallback(OutputCallback callback);
        [DllImport("openpose")] public static extern void OP_SetOutputEnable(bool enable);
        
        /*
         * Send a callback function to openpose logging system. No message will be received if no callback is sent. 
         * The function will be called in op::log() or op::logError().
         * Enable/disable the debug callback. Can be set at runtime.
         */ 
        [DllImport("openpose")] public static extern void OP_RegisterDebugCallback(DebugCallback callback);
        [DllImport("openpose")] public static extern void OP_SetDebugEnable(bool enable);

        /*
         * Enable/disable image output callback. Disable will save some time since data is large. 
         */
        [DllImport("openpose")] public static extern void OP_SetImageOutputEnable(bool enable);

        /*
         * Run openpose if not running. It may take several seconds to fully start. 
         */
        [DllImport("openpose")] public static extern void OP_Run();

        /*
         * Shut down openpose program if it is running. It may take several seconds to fully stop it. 
         */
        [DllImport("openpose")] public static extern void OP_Shutdown();

        /* 
         * Openpose configurations - please read openpose documentation for explanation
         */
        [DllImport("openpose")] public static extern void OP_ConfigurePose(
            bool body_disable,
            int net_resolution_x, int net_resolution_y, // Point
            int output_resolution_x, int output_resolution_y, // Point
            byte keypoint_scale_mode, // ScaleMode
            int num_gpu, int num_gpu_start, int scale_number, float scale_gap,
            byte pose_render_mode, // RenderMode
            byte model_pose, // PoseModel
            bool disable_blending, float alpha_pose, float alpha_heatmap, int part_to_show, string model_folder,
            bool heatmaps_add_parts, bool heatmaps_add_bkg, bool heatmaps_add_PAFs, // vector<HeatMapType> 
            byte heatmap_scale_mode, // ScaleMode
            bool part_candidates, float render_threshold, int number_people_max
        );
        [DllImport("openpose")] public static extern void OP_ConfigureHand(
            bool hand,
            int hand_net_resolution_x, int hand_net_resolution_y, // Point
            int hand_scale_number, float hand_scale_range, bool hand_tracking,
            byte hand_render_mode, // RenderMode
            float hand_alpha_pose, float hand_alpha_heatmap, float hand_render_threshold
        );
        [DllImport("openpose")] public static extern void OP_ConfigureFace(
            bool face, 
            int face_net_resolution_x, int face_net_resolution_y, // Point
            byte face_render_mode, // RenderMode
            float face_alpha_pose, float face_alpha_heatmap, float face_render_threshold
        );
        [DllImport("openpose")] public static extern void OP_ConfigureExtra(
            bool _3d, int _3d_min_views, bool _identification, int _tracking, int _ik_threads
        );
        [DllImport("openpose")] public static extern void OP_ConfigureInput(
            byte producer_type, string producer_string, // ProducerType and string
            ulong frame_first, ulong frame_step, ulong frame_last,
            bool process_real_time, bool frame_flip, int frame_rotate, bool frames_repeat, 
            int camera_resolution_x, int camera_resolution_y, // Point
            double webcam_fps, string camera_parameter_path, bool undistort_image, uint image_directory_stereo
        );
        [DllImport("openpose")] public static extern void OP_ConfigureOutput(            
            double verbose, string write_keypoint, byte write_keypoint_format, // DataFormat
            string write_json, string write_coco_json, string write_coco_foot_json, int write_coco_json_variant,
            string write_images, string write_images_format, string write_video,
            double camera_fps, string write_heatmaps, string write_heatmaps_format, 
            string write_video_adam, string write_bvh, string udp_host, string udp_port
        );
        [DllImport("openpose")] public static extern void OP_ConfigureGui(            
            ushort display_mode, // DisplayMode
            bool gui_verbose, bool full_screen
        );
    }
}
