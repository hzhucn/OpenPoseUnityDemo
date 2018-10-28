using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenPose.Example
{
    public class HumanController2D : MonoBehaviour {

        public int PoseKeypointsCount = 25;
        public int HandKeypointsCount = 21;
        public int FaceKeypointsCount = 70;
        public float ScoreThres = 0.05f;

        [SerializeField] Transform PoseParent;
        [SerializeField] Transform LHandParent;
        [SerializeField] Transform RHandParent;
        [SerializeField] Transform FaceParent;
        private List<Transform> poseJoints = new List<Transform>();
        private List<Transform> lHandJoints = new List<Transform>();
        private List<Transform> rHandJoints = new List<Transform>();
        private List<Transform> faceJoints = new List<Transform>();

        public void DrawHuman(ref OPDatum datum, int bodyIndex){
            if (bodyIndex >= datum.poseKeypoints.GetSize(0)){
                ClearHuman();
            } else {
                DrawBody(ref datum, bodyIndex);
                DrawHand(ref datum, bodyIndex);
                DrawFace(ref datum, bodyIndex);
            }

        }

        public void ClearHuman(){
            PoseParent.gameObject.SetActive(false);
            LHandParent.gameObject.SetActive(false);
            RHandParent.gameObject.SetActive(false);
            FaceParent.gameObject.SetActive(false);
        }

        private void DrawBody(ref OPDatum datum, int bodyIndex){
            if (datum.poseKeypoints == null) {
                PoseParent.gameObject.SetActive(false);
                return;
            } else {
                PoseParent.gameObject.SetActive(true);
            }
            // Pose
            for (int part = 0; part < poseJoints.Count; part++) {
                // Joints overflow
                if (part >= datum.poseKeypoints.GetSize(1)) {
                    poseJoints[part].gameObject.SetActive(false);
                    continue;
                }
                // Compare score
                if (datum.poseKeypoints.Get(bodyIndex, part, 2) < ScoreThres) {
                    poseJoints[part].gameObject.SetActive(false);
                } else {
                    poseJoints[part].gameObject.SetActive(true);
                    Vector3 pos = new Vector3(datum.poseKeypoints.Get(bodyIndex, part, 0), datum.poseKeypoints.Get(bodyIndex, part, 1), 0f);
                    poseJoints[part].localPosition = pos;
                }
            }
        }

        private void DrawHand(ref OPDatum datum, int bodyIndex) {
            if (datum.handKeypoints == null) {
                LHandParent.gameObject.SetActive(false);
                RHandParent.gameObject.SetActive(false);
                return;
            } else {                
                LHandParent.gameObject.SetActive(true);
                RHandParent.gameObject.SetActive(true);
            }
            // Left
            for (int part = 0; part < lHandJoints.Count; part++) {
                // Joints overflow
                if (part >= datum.handKeypoints.left.GetSize(1)) {
                    lHandJoints[part].gameObject.SetActive(false);
                    continue;
                }
                // Compare score
                if (datum.handKeypoints.left.Get(bodyIndex, part, 2) < ScoreThres) {
                    lHandJoints[part].gameObject.SetActive(false);
                } else {
                    lHandJoints[part].gameObject.SetActive(true);
                    Vector3 pos = new Vector3(datum.handKeypoints.left.Get(bodyIndex, part, 0), datum.handKeypoints.left.Get(bodyIndex, part, 1), 0f);
                    lHandJoints[part].localPosition = pos;
                }
            }
            // Right
            for (int part = 0; part < rHandJoints.Count; part++) {
                // Joints overflow
                if (part >= datum.handKeypoints.right.GetSize(1)) {
                    rHandJoints[part].gameObject.SetActive(false);
                    continue;
                }
                // Compare score
                if (datum.handKeypoints.right.Get(bodyIndex, part, 2) < ScoreThres) {
                    rHandJoints[part].gameObject.SetActive(false);
                } else {
                    rHandJoints[part].gameObject.SetActive(true);
                    Vector3 pos = new Vector3(datum.handKeypoints.right.Get(bodyIndex, part, 0), datum.handKeypoints.right.Get(bodyIndex, part, 1), 0f);
                    rHandJoints[part].localPosition = pos;
                }
            }
        }

        private void DrawFace(ref OPDatum datum, int bodyIndex){
            if (datum.faceKeypoints == null) {
                FaceParent.gameObject.SetActive(false);
                return;
            } else {
                FaceParent.gameObject.SetActive(true);
            }
            // Face
            for (int part = 0; part < faceJoints.Count; part++) {
                // Joints overflow
                if (part >= datum.faceKeypoints.GetSize(1)) {
                    faceJoints[part].gameObject.SetActive(false);
                    continue;
                }
                // Compare score
                if (datum.faceKeypoints.Get(bodyIndex, part, 2) < ScoreThres) {
                    faceJoints[part].gameObject.SetActive(false);
                } else {
                    faceJoints[part].gameObject.SetActive(true);
                    Vector3 pos = new Vector3(datum.faceKeypoints.Get(bodyIndex, part, 0), datum.faceKeypoints.Get(bodyIndex, part, 1), 0f);
                    faceJoints[part].localPosition = pos;
                }
            }
        }
        
        // Use this for initialization
        void Start() {
            InitJoints();
        }

        private void InitJoints() {
            // Pose
            if (PoseParent) {
                Debug.Assert(PoseParent.childCount == PoseKeypointsCount, "Pose joint count not match");
                for (int i = 0; i < PoseKeypointsCount; i++) {
                    poseJoints.Add(PoseParent.GetChild(i));
                }
            }
            // LHand
            if (LHandParent) {
                Debug.Assert(LHandParent.childCount == HandKeypointsCount, "LHand joint count not match");
                for (int i = 0; i < HandKeypointsCount; i++) {
                    lHandJoints.Add(LHandParent.GetChild(i));
                }
            }
            // RHand
            if (RHandParent) {
                Debug.Assert(RHandParent.childCount == HandKeypointsCount, "RHand joint count not match");
                for (int i = 0; i < HandKeypointsCount; i++) {
                    rHandJoints.Add(RHandParent.GetChild(i));
                }
            }
            // Face
            if (FaceParent){
                Debug.Assert(FaceParent.childCount == FaceKeypointsCount, "Face joint count not match");
                for (int i = 0; i < FaceKeypointsCount; i++){
                    faceJoints.Add(FaceParent.GetChild(i));
                }
            }
        }
    }
}

