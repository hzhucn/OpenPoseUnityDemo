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
        public Transform KeypointsParent;
        public RectTransform Rect0;
        public float pin = 0.5f;
        public float scaleMultiplier = 1f;
        public float stretchAddition = 0f;

        private RectTransform rectTransform { get { return GetComponent<RectTransform>(); } }
        private Image image { get { return GetComponent<Image>(); } }

        // Update is called once per frame
        void Update() {
            if (mode == Mode.Rect){
                if (KeypointsParent){
                    if (KeypointsParent.gameObject.activeSelf){
                        var childList = KeypointsParent.GetComponentsInChildren<RectTransform>(false);
                        if (childList.Length > 20){ // Use KeypointsParent rect
                            // find rect
                            bool childFlag = false;
                            float xMin = float.PositiveInfinity, yMin = float.PositiveInfinity;
                            float xMax = float.NegativeInfinity, yMax = float.NegativeInfinity;
                            foreach (var t in childList){
                                if (t == KeypointsParent) continue;
                                    childFlag = true;
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
                            if (childFlag) {
                                rectTransform.position = new Vector2((xMin + xMax) / 2f, (yMin + yMax) / 2f); 
                                rectTransform.sizeDelta = new Vector2(xMax - xMin, yMax - yMin) * 1.5f;
                            }
                        } else { // Use Rect0
                            if (Rect0){
                                if (Rect0.gameObject.activeInHierarchy) {
                                    rectTransform.position = Rect0.position;
                                    rectTransform.sizeDelta = Rect0.sizeDelta * 0.8f;
                                }
                            }
                        }                        
                    } else { // Use Joints
                        if (Joint0 && Joint1) {
                            // Pin
                            rectTransform.sizeDelta = Vector2.one * 150f;
                            rectTransform.position = (1f - pin) * Joint0.position + pin * Joint1.position;
                            rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2((Joint0.position - Joint1.position).y, (Joint0.position - Joint1.position).x) * Mathf.Rad2Deg);
                        }
                    }
                }
            } else 
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
