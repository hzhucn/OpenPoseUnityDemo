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

        private LineRenderer lineRenderer { get { return GetComponent<LineRenderer>(); } }

        // Update is called once per frame
        void Update() {
            if (Joint0 && Joint1) {
                lineRenderer.enabled = Joint0.gameObject.activeInHierarchy && Joint1.gameObject.activeInHierarchy;
                lineRenderer.SetPosition(0, Joint0.localPosition);
                lineRenderer.SetPosition(1, Joint1.localPosition);
            }
        }
    }
}
