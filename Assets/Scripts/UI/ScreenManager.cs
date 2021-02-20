using UnityEngine;
using UnityEngine.UIElements;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// This manages the Screen VisualElement
    /// </summary>
    public class ScreenManager : MonoBehaviour
    {
        /// <summary>
        /// RootVisualElement of Screen
        /// </summary>
        public static VisualElement RootVisualElement = null;

        /// <summary>
        /// The container that holds all elements in the screen
        /// </summary>
        public static VisualElement OverallContainer = null;

        private void Awake()
        {
            var screenComponent = GameObject.Find("UI").GetComponent<UIDocument>();

            RootVisualElement = screenComponent.rootVisualElement;

            if (RootVisualElement == null)
            {
                throw new System.Exception("Screen RootVisualElement is null");
            }
            OverallContainer = RootVisualElement.Q("OverallContainer");
            if (OverallContainer == null)
            {
                throw new System.Exception("Screen OverallContainer is null");
            }
        }
    }
}
