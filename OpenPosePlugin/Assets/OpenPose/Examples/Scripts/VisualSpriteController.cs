using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example
{
    public class VisualSpriteController : MonoBehaviour
    {
        public enum Mode
        {
            None, 
            Pin, 
            Stretch, 
            Scale, 
            Rect
        }

        public Mode mode = Mode.Stretch;
        public RectTransform Joint0;
        public RectTransform Joint1;
        public RectTransform Rect0;
        public float pin = 0.5f;
        public float scaleMultiplier = 1f;
        public float stretchAddition = 0f;

        private RectTransform rectTransform { get { return GetComponent<RectTransform>(); } }
        private Image image { get { return GetComponent<Image>(); } }

        // Update is called once per frame
        void Update() {
            if (mode == Mode.Rect){
                if (Rect0){
                    if (Rect0.gameObject.activeInHierarchy) {
                        rectTransform.position = Rect0.position;
                        rectTransform.sizeDelta = Rect0.sizeDelta * 0.8f;
                    } else {
                        if (Joint0 && Joint1) {
                            // Pin
                            rectTransform.position = (1f - pin) * Joint0.position + pin * Joint1.position;
                            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2((Joint0.position - Joint1.position).y, (Joint0.position - Joint1.position).x) * Mathf.Rad2Deg);
                        }
                    }
                }
            } 
            if (Joint0 && Joint1) {
                if (Joint0.gameObject.activeInHierarchy && Joint1.gameObject.activeInHierarchy)
                {
                    image.enabled = true;
                    Vector2 diff = Joint0.position - Joint1.position;
                    switch (mode)
                    {
                        case Mode.None:
                            break;
                        case Mode.Pin:
                            rectTransform.position = (1f - pin) * Joint0.position + pin * Joint1.position;
                            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
                            break;
                        case Mode.Stretch:
                            rectTransform.position = (1f - pin) * Joint0.position + pin * Joint1.position;
                            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
                            rectTransform.sizeDelta = new Vector2((diff.magnitude / rectTransform.localScale.x + stretchAddition), rectTransform.sizeDelta.y);
                            break;
                        case Mode.Scale:
                            rectTransform.position = (1f - pin) * Joint0.position + pin * Joint1.position;
                            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
                            rectTransform.localScale = diff.magnitude * scaleMultiplier * Vector3.one;
                            break;
                    }
                } else
                {
                    image.enabled = false;
                }
            }
        }
    }
}
