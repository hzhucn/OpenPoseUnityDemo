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
		private static bool dataFlag = false;

        // Thread 
        private static Thread opThread;

        # region User functions
        public static void OPEnableDebug(bool enable = true){
            OPAPI.OP_SetDebugEnable(enable);
        }
        public static void OPEnableOutput(bool enable = true){
            OPAPI.OP_SetOutputEnable(enable);
        }
        public static void OPEnableImageOutput(bool enable = true){
            OPAPI.OP_SetImageOutputEnable(enable);
        }
        public static void OPConfigureAllInDefault(){
            OPConfigurePose();
            OPConfigureHand();
            OPConfigureFace();
            OPConfigureExtra();
            OPConfigureInput();
            OPConfigureOutput();
            OPConfigureGui();
        }
        public static void OPRun() {
            if (opThread != null && opThread.IsAlive) {
                Debug.Log("OP already started");
            } else {
                // Start OP thread
                opThread = new Thread(new ThreadStart(OPExecuteThread));
                opThread.Start();
            }
        }
        public static bool OPGetOutput(out OPDatum data){
            data = currentData;
			return dataFlag;
        }
        public static void OPShutdown() {
            OPAPI.OP_Shutdown();
        }
        public static void OPConfigurePose(
            bool body_disable = false, Vector2Int? net_resolution = null, Vector2Int? output_resolution = null,
            ScaleMode keypoint_scale_mode = ScaleMode.InputResolution,
            int num_gpu = -1, int num_gpu_start = 0, int scale_number = 1, float scale_gap = 0.3f,
            RenderMode pose_render_mode = RenderMode.Gpu, PoseModel model_pose = PoseModel.BODY_25,
            bool disable_blending = false, float alpha_pose = 0.6f, float alpha_heatmap = 0.7f,
            int part_to_show = 0, string model_folder = null, 
            HeatMapType heatmap_type = HeatMapType.None, 
            ScaleMode heatmap_scale_mode = ScaleMode.UnsignedChar, 
            bool part_candidates = false, float render_threshold = 0.05f, int number_people_max = -1){
            
            // Other default values
            Vector2Int _net_res = net_resolution ?? new Vector2Int(-1, 320);
            Vector2Int _output_res = output_resolution ?? new Vector2Int(-1, -1);
            model_folder = model_folder ?? Application.streamingAssetsPath + "/models/";

            OPAPI.OP_ConfigurePose(
                body_disable,
                _net_res.x, _net_res.y, // Point
                _output_res.x, _output_res.y, // Point
                (byte) keypoint_scale_mode, // ScaleMode
                num_gpu, num_gpu_start, scale_number, scale_gap,
                (byte) pose_render_mode, // RenderMode
                (byte) model_pose, // PoseModel
                disable_blending, alpha_pose, alpha_heatmap, part_to_show, model_folder,
                Convert.ToBoolean(heatmap_type & HeatMapType.Parts), 
                Convert.ToBoolean(heatmap_type & HeatMapType.Background), 
                Convert.ToBoolean(heatmap_type & HeatMapType.PAFs), // vector<HeatMapType> 
                (byte) heatmap_scale_mode, // ScaleMode
                part_candidates, render_threshold, number_people_max
            );
        }
        public static void OPConfigureHand(
            bool hand = true, Vector2Int? hand_net_resolution = null,
            int hand_scale_number = 1, float hand_scale_range = 0.4f, bool hand_tracking = false,
            RenderMode hand_render_mode = RenderMode.None, 
            float hand_alpha_pose = 0.6f, float hand_alpha_heatmap = 0.7f, float hand_render_threshold = 0.2f){

            // Other default values
            Vector2Int _hand_res = hand_net_resolution ?? new Vector2Int(256, 256);
            
            OPAPI.OP_ConfigureHand(
                hand, _hand_res.x, _hand_res.y, // Point
                hand_scale_number, hand_scale_range, hand_tracking,
                (byte) hand_render_mode, // RenderMode
                hand_alpha_pose, hand_alpha_heatmap, hand_render_threshold
            );
        }
        public static void OPConfigureFace(
            bool face = true, Vector2Int? face_net_resolution = null,
            RenderMode face_render_mode = RenderMode.None, 
            float face_alpha_pose = 0.6f, float face_alpha_heatmap = 0.7f, float face_render_threshold = 0.4f){

            // Other default values
            Vector2Int _face_res = face_net_resolution ?? new Vector2Int(320, 320);
                
            OPAPI.OP_ConfigureFace(
                face, _face_res.x, _face_res.y, // Point
                (byte) face_render_mode, // RenderMode
                face_alpha_pose, face_alpha_heatmap, face_render_threshold
            );
        }
        public static void OPConfigureExtra(
            bool _3d = false, int _3d_min_views = -1, bool _identification = false, int _tracking = -1,	int _ik_threads = 0){
            
            OPAPI.OP_ConfigureExtra(_3d, _3d_min_views, _identification, _tracking, _ik_threads);
        }
        public static void OPConfigureInput(
            ProducerType producer_type = ProducerType.Webcam, string producer_string = "-1",
            ulong frame_first = 0, ulong frame_step = 1, ulong frame_last = ulong.MaxValue,
            bool process_real_time = false, bool frame_flip = false,
            int frame_rotate = 0, bool frames_repeat = false,
            Vector2Int? camera_resolution = null, double webcam_fps = 30.0, 
            string camera_parameter_path = null, 
            bool undistort_image = true, uint image_directory_stereo = 1){

            // Other default values
            Vector2Int _camera_res = camera_resolution ?? new Vector2Int(-1, -1);
            camera_parameter_path = camera_parameter_path ?? Application.streamingAssetsPath + "/models/cameraParameters/";

            OPAPI.OP_ConfigureInput(
                (byte) producer_type, producer_string, // ProducerType and string
                frame_first, frame_step, frame_last,
                process_real_time, frame_flip, frame_rotate, frames_repeat, 
                _camera_res.x, _camera_res.y, // Point
                webcam_fps, camera_parameter_path, undistort_image, image_directory_stereo
            );
        }
        public static void OPConfigureOutput(
            double verbose = -1.0, string write_keypoint = "", DataFormat write_keypoint_format = DataFormat.Yml, 
            string write_json = "", string write_coco_json = "", string write_coco_foot_json = "", int write_coco_json_variant = 1,
            string write_images = "", string write_images_format = "png", string write_video = "",
            double camera_fps = 30.0, string write_heatmaps = "", string write_heatmaps_format = "png", 
            string write_video_adam = "", string write_bvh = "", string udp_host = "", string udp_port = "8051"){
                
            OPAPI.OP_ConfigureOutput(
                verbose, write_keypoint, (byte) write_keypoint_format, // DataFormat
                write_json, write_coco_json, write_coco_foot_json, write_coco_json_variant, 
                write_images, write_images_format, write_video,
                camera_fps, write_heatmaps, write_heatmaps_format, 
                write_video_adam, write_bvh, udp_host, udp_port
            );
        }
        public static void OPConfigureGui(
            DisplayMode display_mode = DisplayMode.NoDisplay, 
            bool gui_verbose = false, bool full_screen = false){

            OPAPI.OP_ConfigureGui(
                (ushort) display_mode, // DisplayMode
                gui_verbose, full_screen
            );
        }
        #endregion
        
		// Log callback
        private static DebugCallback OPLog = delegate(string message, int type){
            switch (type){
                case 0: Debug.Log("OP_Log: " + message); break;
                case -1: Debug.LogError("OP_Error: " + message); break;
                case 1: Debug.LogWarning("OP_Warning: " + message); break;
            }
        };

		// Output callback
        private static OutputCallback OPOutput = delegate(IntPtr ptrPtr, int ptrSize, IntPtr sizePtr, int sizeSize, byte outputType){
            // End of frame signal is received, turn on the flag
            if ((OutputType)outputType == OutputType.None) {
                dataFlag = true;
                return;
            }

			// Safety check
            if (ptrSize < 0 || sizeSize < 0) return;

            // Parse ptrPtr to ptrArray
			var ptrArray = new IntPtr[ptrSize];
            Marshal.Copy(ptrPtr, ptrArray, 0, ptrSize);

            // Parse sizePtr to sizeArray
			var sizeArray = new int[sizeSize];
            Marshal.Copy(sizePtr, sizeArray, 0, sizeSize);

            // Write output to data struct
			OPOutputParser.ParseOutput(ref currentData, ptrArray, sizeArray, (OutputType)outputType);
        };

        // OP thread
        private static void OPExecuteThread() {            
            // Register OP log callback
            OPAPI.OP_RegisterDebugCallback(OPLog);

            // Register OP output callback
            OPAPI.OP_RegisterOutputCallback(OPOutput);

            // Start OP with output callback
            OPAPI.OP_Run();
        }

        private void Start() {
            StartCoroutine(ClearDataFlagCoroutine());
        }

		private IEnumerator ClearDataFlagCoroutine(){
            // Check if data receive finished every frame
            while (true) {
                // New data finished 
                yield return new WaitForEndOfFrame();
                if (dataFlag) {
                    dataFlag = false;
                    currentData = new OPDatum();
                }
            }
		}

        private void OnDestroy() {
            // Stop openpose
            OPShutdown();
        }
	}
}