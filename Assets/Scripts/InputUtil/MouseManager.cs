using System;
using UnityEngine;

namespace WorkstationDesigner.InputUtil
{
    /// <summary>
    /// This class is used to get mouse button states that are blocked by the UI
    /// </summary>
    public class MouseManager : MonoBehaviour
    {
        private const int NUM_MOUSE_BUTTONS = 3;
        private static readonly bool[] mouseButtonStates = new bool[NUM_MOUSE_BUTTONS];
        private static bool mouseOver;

        static MouseManager()
        {
            for (var i = 0; i < NUM_MOUSE_BUTTONS; i++)
            {
                mouseButtonStates[i] = false;
            }
            mouseOver = false;
        }

        public void Update()
        {
            for (var i = 0; i < NUM_MOUSE_BUTTONS; i++)
            {
                if (!Input.GetMouseButton(i))
                {
                    mouseButtonStates[i] = false;
                }
            }
        }

        /// <summary>
        /// Never call this function. Only UIBackdrop should call this function in its event callbacks.
        /// </summary>
        public static void SetMouseButton(int mouseButton, bool state)
        {
            if (mouseButton < 0 || mouseButton >= NUM_MOUSE_BUTTONS)
            {
                throw new Exception("Mouse button out of bounds");
            }
            else
            {
                mouseButtonStates[mouseButton] = state;
            }
        }

        /// <summary>
        /// Get the pressed state of a mouse button, as you would with Unity's Input.GetMouseButton
        /// </summary>
        /// <param name="mouseButton">The index of the mouse button</param>
        /// <returns></returns>
        public static bool GetMouseButton(int mouseButton)
        {

            if (mouseButton < 0 || mouseButton >= NUM_MOUSE_BUTTONS)
            {
                throw new Exception("Mouse button out of bounds");
            }
            else
            {
                return mouseButtonStates[mouseButton];
            }
        }

        /// <summary>
        /// Never call this function. Only UIBackdrop should call this function in its event callbacks.
        /// </summary>
        /// <param name="over">Whether the mouse is over the non-UI region or not</param>
        public static void SetMouseOver(bool over)
        {
            mouseOver = over;
        }

        /// <summary>
        /// Get the pressed state of a mouse button, as you would with Unity's Input.GetMouseButton
        /// </summary>
        /// <returns>Whether the mouse is over the non-UI region</returns>
        public static bool GetMouseOver()
        {
            return mouseOver;
        }
    }
}
