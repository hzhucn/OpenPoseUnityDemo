using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenPose {
	public enum ValType : int {
		Byte = 0,
		Int = 1,
		Long = 2,
		Float = 3, 
		String = 4
	}

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
}
