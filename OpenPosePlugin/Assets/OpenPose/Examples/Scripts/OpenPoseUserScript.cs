using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example {
    /*
     * User of OPWrapper 
     */
    public class OpenPoseUserScript : MonoBehaviour {

        // The 2D human to control
        [SerializeField] Transform humanContainer;
        [SerializeField] ImageRenderer imageRenderer;
        [SerializeField] Text fpsText;
        [SerializeField] Text peopleText;

        // Output control
        private OPDatum datum;

        // User settings
		public ProducerType inputType = ProducerType.Webcam;
		public string producerString = "-1";
        public int maxPeople = -1;
        public float renderThreshold = 0.05f;
        public bool 
            handEnabled = false, 
            faceEnabled = false;
        public Vector2Int 
            netResolution = new Vector2Int(-1, 368), 
            handResolution = new Vector2Int(368, 368), 
            faceResolution = new Vector2Int(368, 368);
        public void SetHandEnabled(bool enabled) { handEnabled = enabled; }
        public void SetFaceEnabled(bool enabled) { faceEnabled = enabled; }
        public void SetRenderThreshold(string s){float res; if (float.TryParse(s, out res)){renderThreshold = res;};}
        public void SetMaxPeople(string s){int res; if (int.TryParse(s, out res)){maxPeople = res;};}
        public void SetPoseResX(string s){int res; if (int.TryParse(s, out res)){netResolution.x = res;};}
        public void SetPoseResY(string s){int res; if (int.TryParse(s, out res)){netResolution.y = res;};}
        public void SetHandResX(string s){int res; if (int.TryParse(s, out res)){handResolution.x = res;};}
        public void SetHandResY(string s){int res; if (int.TryParse(s, out res)){handResolution.y = res;};}
        public void SetFaceResX(string s){int res; if (int.TryParse(s, out res)){faceResolution.x = res;};}
        public void SetFaceResY(string s){int res; if (int.TryParse(s, out res)){faceResolution.y = res;};}

        public void ApplyChanges(){ StartCoroutine(UserRebootOpenPoseCoroutine()); }

        // Bg image
        public bool renderBgImg = false;
        public void ToggleRenderBgImg(){
            renderBgImg = !renderBgImg;
            OPWrapper.OPEnableImageOutput(renderBgImg);
            imageRenderer.FadeInOut(renderBgImg);
        }

        // Number of people
        int numberPeople = 0;

        // Frame rate calculation
        [Range(0f, 1f)] 
        public float frameRateSmoothRatio = 0.8f;
        private float avgFrameRate = 0f;
        private float avgFrameTime = -1f;
        private float lastFrameTime = -1f;

        private void Start() {
            // Configure openpose with default value, 
            //OPWrapper.OPConfigureAllInDefault();
            // or using specific configuration for each
            UserConfigureOpenPose();
            // Enable openpose log to unity (default true)
            OPWrapper.OPEnableDebug(true);
            // Enable openpose output to unity (default true)
            OPWrapper.OPEnableOutput(true);
            // Enable receiving image (default false)
            OPWrapper.OPEnableImageOutput(renderBgImg);
            // Start openpose
            OPWrapper.OPRun();
        }

        // User can change the settings here
        private void UserConfigureOpenPose(){
            OPWrapper.OPConfigurePose(
                /* body_disable */ false, /* net_resolution */ netResolution, /* output_resolution */ null,
                /* keypoint_scale_mode */ ScaleMode.InputResolution,
                /* num_gpu */ -1, /* num_gpu_start */ 0, /* scale_number */ 1, /* scale_gap */ 0.3f,
                /* pose_render_mode */ RenderMode.Gpu, /* model_pose */ PoseModel.BODY_25,
                /* disable_blending */ false, /* alpha_pose */ 0.6f, /* alpha_heatmap */ 0.7f,
                /*t part_to_show */ 0, /* model_folder */ null, 
                /* heatmap_type */ HeatMapType.None, /* heatmap_scale_mode */ ScaleMode.UnsignedChar, 
                /* part_candidates */ false, /* render_threshold */ renderThreshold, /* number_people_max */ maxPeople);

            OPWrapper.OPConfigureHand(
                /* hand */ handEnabled, /* hand_net_resolution */ handResolution, 
                /* hand_scale_number */ 1, /* hand_scale_range */ 0.4f, /* hand_tracking */ false,
                /* hand_render_mode */ RenderMode.None, 
                /* hand_alpha_pose */ 0.6f, /* hand_alpha_heatmap */ 0.7f, /* hand_render_threshold */ 0.2f);

            OPWrapper.OPConfigureFace(
                /* face */ faceEnabled, /* face_net_resolution */ faceResolution,
                /* face_render_mode */ RenderMode.None, 
                /* face_alpha_pose */ 0.6f, /* face_alpha_heatmap */ 0.7f, /* face_render_threshold */ 0.4f);

            OPWrapper.OPConfigureExtra(
                /* _3d */ false, /* _3d_min_views */ -1, /* _identification */ false, /* _tracking */ -1,	/* _ik_threads */ 0);

            OPWrapper.OPConfigureInput(
                /* producer_type */ inputType, /* producer_string */ producerString,
                /* frame_first */ 0, /* frame_step */ 1, /* frame_last */ ulong.MaxValue,
                /* process_real_time */ false, /* frame_flip */ false,
                /* frame_rotate */ 0, /* frames_repeat */ false,
                /* camera_resolution */ null, /* camera_parameter_path */ null, 
                /* undistort_image */ true, /* image_directory_stereo */ 1);

            OPWrapper.OPConfigureOutput(
                /* verbose */ -1.0, /* write_keypoint */ "", /* write_keypoint_format */ DataFormat.Yml, 
                /* write_json */ "", /* write_coco_json */ "", /* write_coco_foot_json */ "", /* write_coco_json_variant */ 1,
                /* write_images */ "", /* write_images_format */ "png", /* write_video */ "",
                /* camera_fps */ 30.0, /* write_heatmaps */ "", /* write_heatmaps_format */ "png", 
                /* write_video_adam */ "", /* write_bvh */ "", /* udp_host */ "", /* udp_port */ "8051");

            OPWrapper.OPConfigureGui(
                /* display_mode */ DisplayMode.NoDisplay, 
                /* gui_verbose */ false, /* full_screen */ false);
        }

        private IEnumerator UserRebootOpenPoseCoroutine() {
            if (OPWrapper.state == OPState.None) yield break;
            // Shutdown if running
            if (OPWrapper.state == OPState.Running) { 
                OPWrapper.OPShutdown();
            }
            // Wait until fully stopped
            yield return new WaitUntil( ()=>{ return OPWrapper.state == OPState.Ready; } ); 
            // Configure and start
            UserConfigureOpenPose();
            OPWrapper.OPRun();
        }

        private void Update() {
            // New data received
            if (OPWrapper.OPGetOutput(out datum)){
                
                // Draw human in data
                int i = 0;
                foreach (var human in humanContainer.GetComponentsInChildren<HumanController2D>()){
                    human.DrawHuman(ref datum, i++, renderThreshold);
                }

                // Draw image
                imageRenderer.UpdateImage(datum.cvInputData);

                // Number of people
                if (datum.poseKeypoints == null || datum.poseKeypoints.Empty()) numberPeople = 0;
                else numberPeople = datum.poseKeypoints.GetSize(0);
                peopleText.text = "People: " + numberPeople;

                // Calculate framerate
                if (lastFrameTime > 0f) {
                    if (avgFrameTime < 0f) avgFrameTime = Time.time - lastFrameTime;
                    else {
                        avgFrameTime = Mathf.Lerp(Time.time - lastFrameTime, avgFrameTime, frameRateSmoothRatio);
                        avgFrameRate = 1f / avgFrameTime;
                    }
                }
                lastFrameTime = Time.time;
                fpsText.text = avgFrameRate.ToString("F1") + " FPS";
            }
        }
    }
}
