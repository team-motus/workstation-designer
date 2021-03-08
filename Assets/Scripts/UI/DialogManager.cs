using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.InputUtil;

namespace WorkstationDesigner.UI
{
    public class DialogManager : MonoBehaviour
    {
        // Assets
        private static VisualTreeAsset dialogAsset = null;
        private static VisualTreeAsset dialogFooterButtonAsset = null;
        private static VisualTreeAsset screenOverlayAsset = null;

        private static VisualElement screenOverlay = null;

        /// <summary>
        /// Represents a dialog
        /// </summary>
        private class Dialog
        {
            /// <summary>
            /// Context associated with what opened this dialog
            /// </summary>
            public object Context;

            /// <summary>
            /// The actual dialog VisualElement
            /// </summary>
            public VisualElement Element { get; private set; }

            /// <summary>
            /// What to do when the Escape button is pressed while the dialog is open
            /// </summary>
            public Action EscAction { get; private set; }

            public Dialog(object context, VisualElement visualElement, Action escAction)
            {
                this.Context = context;
                this.Element = visualElement;
                this.EscAction = escAction;
            }
        }

        /// <summary>
        /// The current open dialog
        /// </summary>
        private static Dialog activeDialog = null;

        /// <summary>
        /// Dictionary of all cached dialogs
        /// </summary>
        private static Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();

        // These values must match those in the UXML/USS files
        private const float DIALOG_WIDTH = 400; // px
        private const float DIALOG_HEIGHT = 400; // px

        public delegate void CustomizeDialogDelegate(VisualElement dialogRootElement);

        void Awake()
        {
            // Load assets

            dialogAsset = Resources.Load<VisualTreeAsset>("UI/Dialog");
            if (dialogAsset == null)
            {
                throw new Exception("dialogAsset is null");
            }

            dialogFooterButtonAsset = Resources.Load<VisualTreeAsset>("UI/DialogFooterButton");
            if (dialogFooterButtonAsset == null)
            {
                throw new Exception("dialogFooterButtonAsset is null");
            }

            screenOverlayAsset = Resources.Load<VisualTreeAsset>("UI/ScreenOverlay");
            if (screenOverlayAsset == null)
            {
                throw new Exception("screenOverlayAsset is null");
            }
        }

        /// <summary>
        /// Check if dialog key already exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(string key)
        {
            return dialogs.ContainsKey(key);
        }

        /// <summary>
        /// Create a new dialog with a given key, body, and list of tuples of footer buttons and callbacks
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dialogBody"></param>
        /// <param name="footerButtons"></param>
        public static void Create(string key, VisualElement dialogBody, Action<object> closeButtonCallback, List<(string, Action<object>)> footerButtons)
        {
            if (ContainsKey(key))
            {
                throw new Exception($"Cannot create dialog with key \"{key}\" since one already exists");
            }

            var element = dialogAsset.CloneTree();

            // Setup dialog X button
            var closeButton = element.Q("dialog-close-button");
            closeButton.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == 0)
                {
                    Close();
                    closeButtonCallback(dialogs[key].Context);
                }
            });

            // Setup dialog body
            var dialogMain = element.Q("dialog-main");
            dialogMain.Add(dialogBody);

            // Setup dialog footer
            var dialogFooter = element.Q("dialog-footer");

            for (var i = 0; i < footerButtons.Count; i++)
            {
                var label = footerButtons[i].Item1;
                var callback = footerButtons[i].Item2;

                var footerButtonContainer = dialogFooterButtonAsset.CloneTree();
                var footerButton = footerButtonContainer.Q("dialog-footer-button");

                var footerButtonLabel = footerButton.Q<Label>("dialog-footer-button-label");
                footerButtonLabel.text = label;

                footerButton.RegisterCallback<MouseDownEvent>(e =>
                {
                    if (e.button == 0)
                    {
                        Close();
                        callback(dialogs[key].Context);
                    }
                });
                dialogFooter.Add(footerButtonContainer);
            }
            dialogs.Add(key, new Dialog(null, element, () =>
            {
                Close();
                closeButtonCallback(dialogs[key].Context);
            }));
        }

        /// <summary>
        /// Close the open dialog
        /// </summary>
        private static void Close()
        {

            if (activeDialog?.Element != null)
            {
                screenOverlay.RemoveFromHierarchy();
                activeDialog?.Element.RemoveFromHierarchy();
                EscManager.PopEscAction(activeDialog?.EscAction);
                activeDialog.Context = null;
                activeDialog = null;
            }
        }

        /// <summary>
        /// Open the dialog with the given key with a given context
        /// </summary>
        /// <param name="key"></param>
        /// <param name="context"></param>
        public static void Open(string key, object context = null, CustomizeDialogDelegate customizeDialog = null)
        {
            if (!ContainsKey(key))
            {
                throw new Exception($"Menu key \"{key}\" does not exist");
            }

            Close();

            activeDialog = dialogs[key];
            activeDialog.Context = context; // Update context

            EscManager.PushEscAction(activeDialog.EscAction);

            if (customizeDialog != null)
            {
                customizeDialog(activeDialog.Element);
            }

            var dialogWindow = activeDialog.Element.Q("dialog-window");

            // Center dialog in screen
            dialogWindow.style.top = (Screen.height / 2 - DIALOG_HEIGHT / 2) * ScreenManager.dpiScaler;
            dialogWindow.style.left = (Screen.width / 2 - DIALOG_WIDTH / 2) * ScreenManager.dpiScaler;

            if(screenOverlay == null)
            {
                // Setup screen overlay to cover back of screen (force user to address the dialog)
                screenOverlay = screenOverlayAsset.CloneTree();
                screenOverlay.style.position = Position.Absolute;
                screenOverlay.style.top = 0;
                screenOverlay.style.left = 0;
                screenOverlay.style.right = 0;
                screenOverlay.style.bottom = 0;
            }
            ScreenManager.OverallContainer.Add(screenOverlay);
            ScreenManager.OverallContainer.Add(activeDialog.Element);
        }
    }
}
