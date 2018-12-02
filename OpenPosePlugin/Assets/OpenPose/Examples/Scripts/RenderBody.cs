using UnityEngine;
using UnityEngine.UI;

namespace OpenPose.Example {
    /*
     * VisualSpriteController controls sprites that visualize human data 2D. 
     * Each sprite shows a certain part of body, connected by Joint0 and Joint1
     */
    public class RenderBody : MonoBehaviour {

        // Bone ends
        public RectTransform Joint0, Joint1;

        // Additional length added to the sprite beyond the length of bone
        public float stretchAddition = 40f;

        private RectTransform rectTransform { get { return GetComponent<RectTransform>(); } }
        private Image image { get { return GetComponent<Image>(); } }

        // Update is called once per frame
        void Update() {
            // Fix the sprite position according to the Joint0 and Joint1
            if (Joint0 && Joint1) {
                if (Joint0.gameObject.activeInHierarchy && Joint1.gameObject.activeInHierarchy) {
                    image.enabled = true;
                    // Set sprite position rotation & size
                    Vector2 diff = Joint0.localPosition - Joint1.localPosition;
                    rectTransform.localPosition = 0.5f * (Joint0.localPosition + Joint1.localPosition);
                    rectTransform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
                    rectTransform.sizeDelta = new Vector2((diff.magnitude / rectTransform.localScale.x + stretchAddition), rectTransform.sizeDelta.y);
                            
                } else {
                    image.enabled = false;
                }
            }
        }
    }
}
