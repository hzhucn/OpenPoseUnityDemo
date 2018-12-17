using System;
using System.Runtime.InteropServices;

namespace OpenPose {
    /*
     * Bind to OpenPose C++ library (openpose.dll)
     * Do not use the functions in this class unless you really understand them
     * Use OPWrapper instead
     */
    public static class OPBind {

        // Output callback delegate
        public delegate void OutputCallback(IntPtr ptrs, int ptrSize, IntPtr sizes, int sizeSize, byte outputType);

        // Logging callback delegate
        public delegate void DebugCallback(string message, int type);

        /*
         * Send a callback function to openpose output. No output will be received if no callback is sent. 
         * Enable/disable the output callback. Can be set at runtime.
         */ 
        [DllImport("openpose")] public static extern void _OPRegisterOutputCallback(OutputCallback callback);
        [DllImport("openpose")] public static extern void _OPSetOutputEnable(bool enable);
        
        /*
         * Send a callback function to openpose logging system. No message will be received if no callback is sent. 
         * The function will be called in op::log() or op::logError().
         * Enable/disable the debug callback. Can be set at runtime.
         */ 
        [DllImport("openpose")] public static extern void _OPRegisterDebugCallback(DebugCallback callback);
        [DllImport("openpose")] public static extern void _OPSetDebugEnable(bool enable);

        /*
         * Enable/disable multi-threading 
         */
        [DllImport("openpose")] public static extern void _OPSetMultiThreadEnable(bool enable);

        /*
         * Enable/disable image output callback. Disable will save some time since data is large. 
         */
        [DllImport("openpose")] public static extern void _OPSetImageOutputEnable(bool enable);

        /*
         * Run openpose if not running. It may take several seconds to fully start. 
         */
        [DllImport("openpose")] public static extern void _OPRun();

        /*
         * Shut down openpose program if it is running. It may take several seconds to fully stop it. 
         */
        [DllImport("openpose")] public static extern void _OPShutdown();

        /* 
         * Openpose configurations - please read openpose documentation for explanation
         */
        [DllImport("openpose")] public static extern void _OPConfigurePose(
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
            bool part_candidates, float render_threshold, int number_people_max, 
            bool maximize_positives, double fps_max
        );
        [DllImport("openpose")] public static extern void _OPConfigureHand(
            bool hand,
            int hand_net_resolution_x, int hand_net_resolution_y, // Point
            int hand_scale_number, float hand_scale_range, bool hand_tracking,
            byte hand_render_mode, // RenderMode
            float hand_alpha_pose, float hand_alpha_heatmap, float hand_render_threshold
        );
        [DllImport("openpose")] public static extern void _OPConfigureFace(
            bool face, 
            int face_net_resolution_x, int face_net_resolution_y, // Point
            byte face_render_mode, // RenderMode
            float face_alpha_pose, float face_alpha_heatmap, float face_render_threshold
        );
        [DllImport("openpose")] public static extern void _OPConfigureExtra(
            bool _3d, int _3d_min_views, bool _identification, int _tracking, int _ik_threads
        );
        [DllImport("openpose")] public static extern void _OPConfigureInput(
            byte producer_type, string producer_string, // ProducerType and string
            ulong frame_first, ulong frame_step, ulong frame_last,
            bool process_real_time, bool frame_flip, int frame_rotate, bool frames_repeat, 
            int camera_resolution_x, int camera_resolution_y, // Point
            string camera_parameter_path, bool undistort_image, uint image_directory_stereo
        );
        [DllImport("openpose")] public static extern void _OPConfigureOutput(            
            double verbose, string write_keypoint, byte write_keypoint_format, // DataFormat
            string write_json, string write_coco_json, string write_coco_foot_json, int write_coco_json_variant,
            string write_images, string write_images_format, string write_video,
            double camera_fps, string write_heatmaps, string write_heatmaps_format, 
            string write_video_adam, string write_bvh, string udp_host, string udp_port
        );
        [DllImport("openpose")] public static extern void _OPConfigureGui(            
            ushort display_mode, // DisplayMode
            bool gui_verbose, bool full_screen
        );
    }
}
