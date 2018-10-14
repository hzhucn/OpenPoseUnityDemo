using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenPose
{
    /*
        Data struct for OP output
        All comments are coppied from OpenPose
     */
    public struct OPDatum{
        public ulong  id; /**< Datum ID. Internally used to sort the Datums if multi-threading is used. */
        public ulong subId; /**< Datum sub-ID. Internally used to sort the Datums if multi-threading is used. */
        public ulong subIdMax; /**< Datum maximum sub-ID. Used to sort the Datums if multi-threading is used. */

        /**
         * Name used when saving the data to disk (e.g. `write_images` or `write_keypoint` flags in the demo).
         */
        public string name;

        /**
         * Corresponding frame number.
         * If the producer (e.g., video) starts from frame 0 and does not repeat any frame, then frameNumber should
         * match the field id.
         */
        public ulong frameNumber;

        // --------------------------- Input image and rendered version parameters ---------------------------- //
        // Not enabled now

        // ------------------------------ Resulting Array<float> data parameters ------------------------------ //
        /**
         * Body pose (x,y,score) locations for each person in the image.
         * It has been resized to the desired output resolution (e.g. `resolution` flag in the demo).
         * Size: #people x #body parts (e.g. 18 for COCO or 15 for MPI) x 3 ((x,y) coordinates + score)
         */
        public List<List<Vector3>> poseKeypoints;

        /**
         * People ID
         * It returns a person ID for each body pose, providing temporal consistency. The ID will be the same one
         * for a person across frames. I.e. this ID allows to keep track of the same person in time.
         * If either person identification is disabled or poseKeypoints is empty, poseIds will also be empty.
         * Size: #people
         */
        public List<long> poseIds;

        /**
         * Body pose global confidence/score for each person in the image.
         * It does not only consider the score of each body keypoint, but also the score of each PAF association.
         * Optimized for COCO evaluation metric.
         * It will highly penalyze people with missing body parts (e.g. cropped people on the borders of the image).
         * If poseKeypoints is empty, poseScores will also be empty.
         * Size: #people
         */
        public List<float> poseScores;

        /**
         * Body pose heatmaps (body parts, background and/or PAFs) for the whole image.
         * This parameter is by default empty and disabled for performance. Each group (body parts, background and
         * PAFs) can be individually enabled.
         * #heatmaps = #body parts (if enabled) + 1 (if background enabled) + 2 x #PAFs (if enabled). Each PAF has 2
         * consecutive channels, one for x- and one for y-coordinates.
         * Order heatmaps: body parts + background (as appears in POSE_BODY_PART_MAPPING) + (x,y) channel of each PAF
         * (sorted as appears in POSE_BODY_PART_PAIRS). See `pose/poseParameters.hpp`.
         * The user can choose the heatmaps normalization: ranges [0, 1], [-1, 1] or [0, 255]. Check the
         * `heatmaps_scale` flag in {OpenPose_path}doc/demo_overview.md for more details.
         * Size: #heatmaps x output_net_height x output_net_width
         */
        public List<float> poseHeatMaps;

        /**
         * Body pose candidates for the whole image.
         * This parameter is by default empty and disabled for performance. It can be enabled with `candidates_body`.
         * Candidates refer to all the detected body parts, before being assembled into people. Note that the number
         * of candidates is equal or higher than the number of body parts after being assembled into people.
         * Size: #body parts x min(part candidates, POSE_MAX_PEOPLE) x 3 (x,y,score).
         * Rather than vector, it should ideally be:
         * std::array<std::vector<std::array<float,3>>, #BP> poseCandidates;
         */
        public List<List<Vector3>> poseCandidates;

        /**
         * Face detection locations (x,y,width,height) for each person in the image.
         * It is resized to cvInputData.size().
         * Size: #people
         */
        public List<Rect> faceRectangles;

        /**
         * Face keypoints (x,y,score) locations for each person in the image.
         * It has been resized to the same resolution as `poseKeypoints`.
         * Size: #people x #face parts (70) x 3 ((x,y) coordinates + score)
         */
        public List<float> faceKeypoints;

        /**
         * Face pose heatmaps (face parts and/or background) for the whole image.
         * Analogous of bodyHeatMaps applied to face. However, there is no PAFs and the size is different.
         * Size: #people x #face parts (70) x output_net_height x output_net_width
         */
        public List<float> faceHeatMaps;

        /**
         * Hand detection locations (x,y,width,height) for each person in the image.
         * It is resized to cvInputData.size().
         * Size: #people
         */
        public List<Pair<Rect>> handRectangles;

        /**
         * Hand keypoints (x,y,score) locations for each person in the image.
         * It has been resized to the same resolution as `poseKeypoints`.
         * handKeypoints[0] corresponds to left hands, and handKeypoints[1] to right ones.
         * Size each Array: #people x #hand parts (21) x 3 ((x,y) coordinates + score)
         */
        public Pair<List<float>> handKeypoints;

        /**
         * Hand pose heatmaps (hand parts and/or background) for the whole image.
         * Analogous of faceHeatMaps applied to face.
         * Size each Array: #people x #hand parts (21) x output_net_height x output_net_width
         */
        public Pair<List<float>> handHeatMaps;

        // ---------------------------------------- 3-D Reconstruction parameters ---------------------------------------- //
        /**
         * Body pose (x,y,z,score) locations for each person in the image.
         * Size: #people x #body parts (e.g. 18 for COCO or 15 for MPI) x 4 ((x,y,z) coordinates + score)
         */
        public List<float> poseKeypoints3D;

        /**
         * Face keypoints (x,y,z,score) locations for each person in the image.
         * It has been resized to the same resolution as `poseKeypoints3D`.
         * Size: #people x #face parts (70) x 4 ((x,y,z) coordinates + score)
         */
        public List<float> faceKeypoints3D;

        /**
         * Hand keypoints (x,y,z,score) locations for each person in the image.
         * It has been resized to the same resolution as `poseKeypoints3D`.
         * handKeypoints[0] corresponds to left hands, and handKeypoints[1] to right ones.
         * Size each Array: #people x #hand parts (21) x 4 ((x,y,z) coordinates + score)
         */
        public Pair<List<float>> handKeypoints3D;

        /**
         * 3x4 camera matrix of the camera (equivalent to cameraIntrinsics * cameraExtrinsics).
         */
        public Matrix4x4 cameraMatrix;

        /**
         * 3x4 extrinsic parameters of the camera.
         */
        public Matrix4x4 cameraExtrinsics;

        /**
         * 3x3 intrinsic parameters of the camera.
         */
        public Matrix4x4 cameraIntrinsics;
    }

    public class Pair<T> : List<T>{
        // Constructors
        public Pair() : base(2){}
        public Pair(IEnumerable<T> collection) : base(collection){
            if (Count > 2) this.RemoveRange(2, Count - 2);
        }
        public Pair(T left, T right) : base(2){
            this.left = left;
            this.right = right;
        }
        private Pair(int capacity){}

        // Access the left element [0]
        public T left {
            get { return this[0]; } 
            set { this[0] = value; }
        }

        // Access the right element [1]
        public T right {
            get { return this[1]; } 
            set { this[1] = value; }
        }
    }

    public class MultiArray<T> : List<T>{
        // Constructors
        public MultiArray() : base(){ Resize(); }
        public MultiArray(params int[] sizes) : base(){ Resize(sizes); }
        public MultiArray(IEnumerable<T> collection, params int[] sizes) : base(collection){ Resize(sizes); }
        private MultiArray(int capacity){}
        private MultiArray(IEnumerable<T> collection){}

        public void Resize(params int[] sizes){
            this.sizes = new List<int>(sizes);            
            if (sizes.Length == 0) volume = 0;
            else {
                volume = 1;
                foreach (var i in sizes){ volume *= i; }
            }
            Capacity = volume;
        }

        public List<int> GetSize(){
            return new List<int>(sizes);
        }
        public int GetSize(int layer){
            if (layer < 0 || layer >= sizes.Count) return 0;
            return sizes[layer];
        }

        public int GetNumberDimensions(){
            return sizes.Count;
        }

        public int GetVolume(){
            return volume;
        }

        // Members
        private List<int> sizes;
        private int volume;
    }

    /*
    Data for a frame. 
    units: List of person detected
     */
    [Serializable]
    public class OPFrame
    {
        public List<OPUnit> units = new List<OPUnit>();

        public static OPFrame FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<OPFrame>(json);
            } catch(Exception err)
            {
                Debug.LogError(err);
                Debug.Log(json);
                return null;
            }
        }
    }

    /*
    Data for a person in a frame
    poseKeypoints: info of keypoints on body, including the screen position and confidence
    handKeypoints: info of keypoints on hands (L/R), including the screen position and confidence
    faceKeypoints: info of keypoints on face, including the screen position and confidence
     */
    [Serializable]
    public class OPUnit
    {
        public List<Vector3> poseKeypoints = new List<Vector3>(); // (x, y) screen position, z stands for confidence (0 - 1)
        public List<Vector3> handKeypoints_L = new List<Vector3>(); // (x, y) screen position, z stands for confidence (0 - 1)
        public List<Vector3> handKeypoints_R = new List<Vector3>(); // (x, y) screen position, z stands for confidence (0 - 1)
        public List<Vector3> faceKeypoints = new List<Vector3>(); // (x, y) screen position, z stands for confidence (0 - 1)
    }

    /*
    Data for an image
    NOT IN USE currently
     */
    [Serializable]
    public class OPImage
    {
        [SerializeField] Vector2Int size;
        [SerializeField] List<Vector3Int> pixels = new List<Vector3Int>();

        public int w { get { return size.x; } }
        public int h { get { return size.y; } }

        public Color GetColorAt(int x, int y)
        {
            if (x >= w || y >= h) return Color.black;

            Debug.Assert(pixels.Count == w * h);
            Vector3Int res = pixels[y * w + x];
            return new Color(res.x / 255f, res.y / 255f, res.z / 255f);
        }

        public static OPImage FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<OPImage>(json);
            }
            catch (Exception err)
            {
                Debug.LogError(err);
                Debug.Log(json);
                return null;
            }
        }
    }
}
