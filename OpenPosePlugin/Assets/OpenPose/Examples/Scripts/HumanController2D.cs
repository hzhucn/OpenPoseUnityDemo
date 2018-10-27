using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenPose.Example
{
    public class HumanController2D : MonoBehaviour {

        public int PoseKeypointsCount = 25;
        public int HandKeypointsCount = 21;
        public float ScoreThres = 0.05f;

        [SerializeField] Transform PoseParent;
        [SerializeField] Transform LHandParent;
        [SerializeField] Transform RHandParent;
        private List<Transform> poseJoints = new List<Transform>();
        private List<Transform> lHandJoints = new List<Transform>();
        private List<Transform> rHandJoints = new List<Transform>();

        public void DrawHuman(ref OPDatum datum, int bodyIndex, bool hand = true, bool face = true){
            if (datum.poseKeypoints == null || bodyIndex >= datum.poseKeypoints.GetSize(0)){
                ClearHuman();
            } else {
                DrawBody(ref datum, bodyIndex);
                if (hand) DrawHand(ref datum, bodyIndex);
                //if (face) DrawFace(ref datum, bodyIndex);
            }

        }

        public void ClearHuman(){
            for (int part = 0; part < poseJoints.Count; part++) {
                poseJoints[part].gameObject.SetActive(false);
            }
        }

        private void DrawBody(ref OPDatum datum, int bodyIndex){
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
            bool invalid = datum.handKeypoints == null;
            // Left
            for (int part = 0; part < lHandJoints.Count; part++) {
                // Joints overflow
                if (invalid || part >= datum.handKeypoints.left.GetSize(1)) {
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
                if (invalid || part >= datum.handKeypoints.right.GetSize(1)) {
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
            // TODO
        }
        
        // Use this for initialization
        void Start()
        {
            InitJoints();
        }

        private void InitJoints() {
            if (PoseParent != null) {
                Debug.Assert(PoseParent.childCount == PoseKeypointsCount);
                if (PoseParent.childCount == PoseKeypointsCount) {
                    for (int i = 0; i < PoseParent.childCount; i++) {
                        poseJoints.Add(PoseParent.GetChild(i));
                    }
                }                
            }
            if (LHandParent != null) {
                Debug.Assert(LHandParent.childCount == HandKeypointsCount);
                if (LHandParent.childCount == HandKeypointsCount) {
                    for (int i = 0; i < LHandParent.childCount; i++) {
                        lHandJoints.Add(LHandParent.GetChild(i));
                    }
                }
            }
            if (RHandParent != null) {
                Debug.Assert(RHandParent.childCount == HandKeypointsCount);
                if (RHandParent.childCount == HandKeypointsCount) {
                    for (int i = 0; i < RHandParent.childCount; i++) {
                        rHandJoints.Add(RHandParent.GetChild(i));
                    }
                }
            }
        }        
    }
}

