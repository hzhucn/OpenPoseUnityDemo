using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example {
    public class VisualSpriteController : MonoBehaviour {

        public RectTransform Joint0;
        public RectTransform Joint1;
        public float stretchAddition = 40f;

        private RectTransform rectTransform { get { return GetComponent<RectTransform>(); } }
        private Image image { get { return GetComponent<Image>(); } }

        // Update is called once per frame
        void Update() {
            if (Joint0 && Joint1) {
                if (Joint0.gameObject.activeInHierarchy && Joint1.gameObject.activeInHierarchy) {
                    image.enabled = true;

                    Vector2 diff = Joint0.position - Joint1.position;
                    rectTransform.position = 0.5f * (Joint0.position + Joint1.position);
                    rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
                    rectTransform.sizeDelta = new Vector2((diff.magnitude / rectTransform.localScale.x + stretchAddition), rectTransform.sizeDelta.y);
                            
                } else
                {
                    image.enabled = false;
                }
            }
        }
    }
}
