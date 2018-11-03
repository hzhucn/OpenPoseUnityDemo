using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OpenPose {
	public static class OPOutputParser {

		public static void ParseOutput(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray, OutputType type){
			switch (type){
				case OutputType.PoseKeypoints: ParsePoseKeypoints(ref datum, ptrArray, sizeArray); break;
				case OutputType.HandKeypoints: ParseHandKeypoints(ref datum, ptrArray, sizeArray); break;
				case OutputType.FaceKeypoints: ParseFaceKeypoints(ref datum, ptrArray, sizeArray); break;
				case OutputType.FaceRectangles: ParseFaceRectangles(ref datum, ptrArray, sizeArray); break;
				default: Debug.Log("Output type not supported yet: " + type); break;
			}
		}

		public static void ParsePoseKeypoints(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray){
			Debug.AssertFormat(ptrArray.Length == 1, "PoseKeypoints array length invalid");
			Debug.AssertFormat(sizeArray.Length == 3, "PoseKeypoints size length invalid");
            int volume = 1;
            foreach(var i in sizeArray){ volume *= i; }
			if (volume == 0) return;
			
			var valArray = new float[volume];
			Marshal.Copy(ptrArray[0], valArray, 0, volume);
			datum.poseKeypoints = new MultiArray<float>(valArray, sizeArray);
		}

		public static void ParsePoseIds(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray){
			
		}

		public static void ParseHandKeypoints(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray){
			Debug.AssertFormat(ptrArray.Length == 2, "HandKeypoints array length invalid");
			Debug.AssertFormat(sizeArray.Length == 3, "PoseKeypoints size length invalid");
			int volume = 1;
            foreach(var i in sizeArray){ volume *= i; }
			if (volume == 0) return;
			
			// Left 
			var valArrayL = new float[volume];
			Marshal.Copy(ptrArray[0], valArrayL, 0, volume);
			var handKeypointsL = new MultiArray<float>(valArrayL, sizeArray);
			// Right
			var valArrayR = new float[volume];
			Marshal.Copy(ptrArray[1], valArrayR, 0, volume);
			var handKeypointsR = new MultiArray<float>(valArrayR, sizeArray);

			datum.handKeypoints = new Pair<MultiArray<float>>(handKeypointsL, handKeypointsR);
		}

		public static void ParseFaceKeypoints(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray){
			Debug.AssertFormat(ptrArray.Length == 1, "FaceKeypoints array length invalid");
			Debug.AssertFormat(sizeArray.Length == 3, "FaceKeypoints size length invalid");
            int volume = 1;
            foreach(var i in sizeArray){ volume *= i; }
			if (volume == 0) return;
			
			var valArray = new float[volume];
			Marshal.Copy(ptrArray[0], valArray, 0, volume);
			datum.faceKeypoints = new MultiArray<float>(valArray, sizeArray);
		}

		public static void ParseFaceRectangles(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray){
			Debug.AssertFormat(ptrArray.Length == 1, "FaceRect array length is invalid");
			Debug.AssertFormat(sizeArray.Length == 2, "FaceRect size length is invalid");
            int volume = 1;
            foreach(var i in sizeArray){ volume *= i; }
			if (volume == 0) return;
			
			var valArray = new float[volume];
			Marshal.Copy(ptrArray[0], valArray, 0, volume);

			var list = new List<Rect>();
			for (int i = 0; i < sizeArray[0]; i++){
				list.Add(new Rect(valArray[i * 4 + 0], valArray[i * 4 + 1], valArray[i * 4 + 2], valArray[i * 4 + 3]));
			}
			
			datum.faceRectangles = list;
		}
	}
}
