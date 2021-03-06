﻿using UnityEngine.UIElements;
using WorkstationDesigner.InputUtil;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// This is a special VisualElement that is used to catch mouse clicks that aren't blocked by the rest of the UI
    /// </summary>
    public class UIBackdrop : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<UIBackdrop, UxmlTraits> { }

        public UIBackdrop()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        void OnGeometryChange(GeometryChangedEvent evt)
        {
            RegisterCallback<MouseDownEvent>(e => MouseManager.SetMouseButton(e.button, true)); // Set mouse button pressed when it's pressed on this VisualElement

            RegisterCallback<MouseOverEvent>(e => MouseManager.SetMouseOver(true));
            RegisterCallback<MouseOutEvent>(e => MouseManager.SetMouseOver(false));

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

    }
}
