using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WorkstationDesigner.InputUtil
{
    /// <summary>
    /// Manages what happens when the Escape button is pressed
    /// </summary>
    public class EscManager: MonoBehaviour
    {
        /// <summary>
        /// Stack of escape actions to perform
        /// </summary>
        private static Stack<Action> escActions = new Stack<Action>();

        /// <summary>
        /// Add an action to perform when Escape is pressed
        /// </summary>
        /// <param name="escAction"></param>
        public static void PushEscAction(Action escAction)
        {
            escActions.Push(escAction);
        }


        /// <summary>
        /// Remove an action to perform when Escape is pressed
        /// </summary>
        /// <param name="escAction"></param>
        public static void PopEscAction(Action escAction)
        {
            if (escActions.Count > 0 && escActions.Peek() == escAction)
            {
                escActions.Pop();
            }
        }

        public void Update()
        {
            // Perform escape action when Escape is pressed
            if (escActions.Count > 0 && Keyboard.current[Key.Escape].wasPressedThisFrame)
            {
                escActions.Pop()();
            }
        }
    }
}
