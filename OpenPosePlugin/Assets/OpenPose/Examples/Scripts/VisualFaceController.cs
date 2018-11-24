using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example {
	public class VisualFaceController : MonoBehaviour {

		// Face center joint (nose)
		[SerializeField] RectTransform faceCenter;

		// Parent of face keypoints
		[SerializeField] Transform keypointsParent;

		// Face rectangle
		[SerializeField] RectTransform faceRect;
		
        private RectTransform rectTransform { get { return GetComponent<RectTransform>(); } }
        private Image image { get { return GetComponent<Image>(); } }

		private bool findKeypointsRect(RectTransform[] keypoints, out Rect rect){
            bool res = false;
            float xMin = float.PositiveInfinity, yMin = float.PositiveInfinity;
            float xMax = float.NegativeInfinity, yMax = float.NegativeInfinity;
            foreach (var t in keypoints){
                if (t == keypointsParent) continue;
                res = true;
                if (t.position.x < xMin) {
                    xMin = t.position.x;
                }
                if (t.position.x > xMax) {
                    xMax = t.position.x;
                }
                if (t.position.y < yMin) {
                    yMin = t.position.y;
                }
                if (t.position.y > yMax) {
                    yMax = t.position.y;
                }
            }
            rect = new Rect();
            rect.xMin = xMin; rect.xMax = xMax; 
            rect.yMin = yMin; rect.yMax = yMax;
            return res;
		}
		
		// Update is called once per frame
		void Update () {
			if (keypointsParent){
                // Face enabled
                if (keypointsParent.gameObject.activeSelf){
                    var childList = keypointsParent.GetComponentsInChildren<RectTransform>(false);
                    // If >= 20 keypoints detected, draw face using keypoints rect. 
                    if (childList.Length >= 20){
                        Rect rect;
                        if (findKeypointsRect(childList, out rect)) {
                            image.enabled = true;
                            rectTransform.position = rect.center; 
                            rectTransform.sizeDelta = rect.size * 1.5f;
                        }
                    } 
                    // Less than 20 keypoints detected, draw face using faceRectangle
                    else {
                        if (faceRect){
                            if (faceRect.gameObject.activeInHierarchy) {
                                image.enabled = true;
                                rectTransform.position = faceRect.position;
                                rectTransform.sizeDelta = faceRect.sizeDelta * 0.8f;
                            } else {                                
                                image.enabled = false;
                            }
                        }
                    }                        
                } 
                // Face disabled, draw face using center joint (nose)
                else {
                    if (faceCenter) {
                        image.enabled = faceCenter.gameObject.activeInHierarchy;
                        rectTransform.sizeDelta = Vector2.one * 150f;
                        rectTransform.position = faceCenter.position;
                        //rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2((faceCenter.position - faceCenter.position).y, (faceCenter.position - faceCenter.position).x) * Mathf.Rad2Deg);
                    }
                }
            }
		}
	}
}
