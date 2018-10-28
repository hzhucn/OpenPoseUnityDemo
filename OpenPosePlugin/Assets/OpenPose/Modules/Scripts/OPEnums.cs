using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenPose {
	public enum OutputType : int {
		None = 0, 
		Ids = 1, 
		Name = 2, 
		FrameNumber = 3, 
		PoseKeypoints = 4, 
		PoseIds = 5, 
		PoseScores = 6, 
		PoseHeatMaps = 7,
		PoseCandidates = 8, 
		FaceRectangles = 9, 
		FaceKeypoints = 10, 
		FaceHeatMaps = 11, 
		HandRectangles = 12, 
		HandKeypoints = 13, 
		HandHeightMaps = 14, 
		PoseKeypoints3D = 15, 
		FaceKeypoints3D = 16, 
		HandKeypoints3D = 17, 
		CameraMatrix = 18, 
		CameraExtrinsics = 19, 
		CameraIntrinsics = 20
	};

	public enum ProducerType : byte {
        /** Stereo FLIR (Point-Grey) camera reader. Based on Spinnaker SDK. */
        FlirCamera,
        /** An image directory reader. It is able to read images on a folder with a interface similar to the OpenCV
         * cv::VideoCapture.
         */
        ImageDirectory,
        /** An IP camera frames extractor, extending the functionality of cv::VideoCapture. */
        IPCamera,
        /** A video frames extractor, extending the functionality of cv::VideoCapture. */
        Video,
        /** A webcam frames extractor, extending the functionality of cv::VideoCapture. */
        Webcam,
        /** No type defined. Default state when no specific Producer has been picked yet. */
        None,
    };
}
