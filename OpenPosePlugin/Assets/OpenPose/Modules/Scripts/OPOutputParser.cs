using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OpenPose {
	public static class OPOutputParser {

		public static void ParseOutput(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray, OutputType type){
			switch (type){
				case OutputType.PoseKeypoints: ParsePoseKeypoints(ref datum, ptrArray, sizeArray); break;
				case OutputType.HandKeypoints: ParseHandKeypoints(ref datum, ptrArray, sizeArray); break;
				default: break;
			}
		}

		public static void ParsePoseKeypoints(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray){
			Debug.AssertFormat(ptrArray.Length == 1, "PoseKeypoints array length is not 1");
            int volume = 1;
            foreach(var i in sizeArray){ volume *= i; }
			if (volume == 0) return;
			
			var valArray = new float[volume];
			Marshal.Copy(ptrArray[0], valArray, 0, volume);
			datum.poseKeypoints = new MultiArray<float>(valArray, sizeArray);
		}

		public static void ParseHandKeypoints(ref OPDatum datum, IntPtr[] ptrArray, int[] sizeArray){
			Debug.AssertFormat(ptrArray.Length == 2, "HandKeypoints array length is not 2");
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
	}
}
