using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example {
	public class ImageRenderer : MonoBehaviour {

		// Texture to be rendered in image
		private Texture2D texture;

		private RawImage image { get { return GetComponent<RawImage>(); } }

		public void UpdateImage(ref OPDatum datum){
			var data = datum.cvInputData; // width * height * 3 
			if (data == null || data.Empty()) return;
			int height = data.GetSize(0), width = data.GetSize(1);
			
			/* Unity does not support BGR24 yet, which is the data representation in OpenCV */
			/* Here we are using RGB24 as data format, then swap R and B in shader */
			texture.Resize(width, height, TextureFormat.RGB24, false);
			texture.LoadRawTextureData(data.ToArray());
			texture.Apply();			
		}

		public void FadeInOut(bool renderImage, float duration = 0.5f){
			if (renderImage) StartCoroutine(FadeCoroutine(Color.white, duration));
			else StartCoroutine(FadeCoroutine(Color.clear, duration));
		}

		private IEnumerator FadeCoroutine(Color goal, float duration){
			Color current = image.color;
			float t = 0f;
			while (t < duration){
				image.color = Color.Lerp(current, goal, t / duration);
				t += Time.deltaTime;
				yield return null;
			}
			image.color = goal;
		}

		// Use this for initialization
		void Start () {
			texture = new Texture2D(1280, 720); 
			image.texture = texture;
		}
	}
}
