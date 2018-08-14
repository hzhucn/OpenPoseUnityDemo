using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace op.examples
{
    public class VisualSpriteController : MonoBehaviour
    {
        public enum Mode
        {
            None, 
            Pin, 
            Stretch, 
            Scale
        }

        public Mode mode = Mode.Stretch;
        public RectTransform Joint0;
        public RectTransform Joint1;
        public float pin = 0.5f;
        public float scaleMultiplier = 1f;
        public float stretchAddition = 0f;

        private RectTransform rectTransform { get { return GetComponent<RectTransform>(); } }
        private Image image { get { return GetComponent<Image>(); } }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {            
            if (Joint0.gameObject.activeSelf && Joint1.gameObject.activeSelf)
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
                        rectTransform.sizeDelta = new Vector2((diff.magnitude + stretchAddition) / rectTransform.localScale.x, rectTransform.sizeDelta.y);
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
