using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example {
	public class ImageRenderer : MonoBehaviour {

		private Texture2D texture;

		public void UpdateImage(ref OPDatum datum){
			var data = datum.cvInputData;
			int height = data.GetSize(0), width = data.GetSize(1);
			texture = new Texture2D(width, height);
			Color32[] texColor = new Color32[width * height];
			for (int i = 0; i < height * width; i++){
				int idx = 3 * i;
				texColor[i] = new Color32(data[idx + 2], data[idx + 1], data[idx], 255);
			}
			texture.SetPixels32(texColor);
			texture.Apply();
			GetComponent<RawImage>().texture = texture;
		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
