﻿using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.Scripts
{
    /// <summary>
    /// Moves associated transform according to user input
    /// 
    /// WASD, Space/LeftShift, and panning with cursor
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        private const float TRANSLATE_SPEED_SCALAR = 10; // Experimentally chosen
        private const float MOUSE_SENSITIVITY_SCALAR = 0.2f; // Experimentally chosen
        private const float CURSOR_INERTIAL_SCALAR = 0.3f; // Experimentally chosen
        private const float MINIMUM_HEIGHT = 0.5f; // Experimentally chosen

        private const float MAXIMUM_DISTNACE_FROM_ORIGIN = 150f; // Experimentally chosen

        public bool translationEnabled = true;
        public bool rotationEnabled = true;
        public bool movementEnabled = true;

        private Vector3 translateVector;
        private Vector2 cursorMotion;
        private Vector2 lastCursorMotion;

        /// <summary>
        /// Provess user input into transform movement
        /// </summary>
        public void Update()
        {
            if (movementEnabled && CameraSwitcher.getEditorCameraStatus())
            {
                if (rotationEnabled && UpdateCursorMotion())
                {
                    Quaternion new_rotation = transform.rotation * Quaternion.AngleAxis(cursorMotion.y, Vector3.left); // Calculate rotation after vertical rotation
                    Vector3 new_forward = new_rotation * Vector3.forward; // Calculare what the new forward direction will be

                    // Prevent rotating vertically past looking directly up or down
                    // If either the X or Z components of the forward vector change, then we know the rotation has overshot
                    if (MathUtil.SameSign(new_forward.x, transform.forward.x) && MathUtil.SameSign(new_forward.z, transform.forward.z))
                    {
                        transform.rotation = new_rotation; // Rotate vertically
                    }

                    transform.Rotate(Vector3.up, cursorMotion.x, Space.World); // Rotate horizontally
                }
                if (translationEnabled && UpdateTranslateVector())
                {
                    // Calculate new position from current position and time, speed, and direction of movement
                    Vector3 new_position = transform.position + Time.deltaTime * TRANSLATE_SPEED_SCALAR * translateVector;

                    // Set transform position but clamp Y to MINIMUM_HEIGHT
                    var newPosition = new Vector3(new_position.x, Mathf.Max(MINIMUM_HEIGHT, new_position.y), new_position.z);
                    if (newPosition.magnitude <= MAXIMUM_DISTNACE_FROM_ORIGIN)
                    {
                        transform.position = newPosition;
                    }
                }
            }
        }

        /// <summary>
        /// Update translate vector from user input
        /// </summary>
        /// <returns>True if the translate vector is non-zero</returns>
        private bool UpdateTranslateVector()
        {
            translateVector = Vector3.zero;
            bool updated = false;

            // Use planar forward instead of transform forward since we don't want to move up or down with WASD
            Vector3 planar_forward = transform.forward;
            planar_forward = new Vector3(planar_forward.x, 0, planar_forward.z).normalized;

            if (Keyboard.current[Key.W].isPressed)
            {
                translateVector += planar_forward;
                updated = true;
            }
            if (Keyboard.current[Key.S].isPressed)
            {
                translateVector -= planar_forward;
                updated = true;
            }
            if (Keyboard.current[Key.A].isPressed)
            {
                translateVector += Vector3.Cross(planar_forward, Vector3.up); // Calculate right direction from cross between forward and up
                updated = true;
            }
            if (Keyboard.current[Key.D].isPressed)
            {
                translateVector -= Vector3.Cross(planar_forward, Vector3.up);
                updated = true;
            }
            if (Keyboard.current[Key.Space].isPressed)
            {
                translateVector += Vector3.up;
                updated = true;
            }
            if (Keyboard.current[Key.LeftShift].isPressed)
            {
                translateVector -= Vector3.up;
                updated = true;
            }

            if (updated) // Only normalize if a button was pressed
            {
                translateVector = translateVector.normalized; // Normalize vector to make movement speed consistent
            }

            return updated;
        }

        /// <summary>
        /// Update cursor motion
        /// </summary>
        /// <returns>True if cursor was moved</returns>
        private bool UpdateCursorMotion()
        {
            if (MouseManager.GetMouseButton(0) && Camera.main != null && Camera.main.pixelRect.Contains(Mouse.current.position.ReadValue()))
            {
                cursorMotion = -Mouse.current.delta.ReadValue() * MOUSE_SENSITIVITY_SCALAR;
                // cursorMotion.y *= -1;

                // Add an intertial effect to camera movement
                if (cursorMotion.x != 0 && MathUtil.SameSign(cursorMotion.x, lastCursorMotion.x))
                {
                    cursorMotion.x += lastCursorMotion.x * CURSOR_INERTIAL_SCALAR;
                }
                if (cursorMotion.y != 0 && MathUtil.SameSign(cursorMotion.y, lastCursorMotion.y))
                {
                    cursorMotion.y += lastCursorMotion.y * CURSOR_INERTIAL_SCALAR;
                }

                lastCursorMotion = cursorMotion;

                return cursorMotion.magnitude > float.Epsilon;
            }
            else
            {
                cursorMotion = Vector2.zero;
                lastCursorMotion = Vector2.zero;
                return false;
            }
        }
    }
}
