﻿using UnityEngine;
using UnityEngine.UIElements;

namespace WorkstationDesigner
{
    public class TitleScreenManager : VisualElement
    {
        public static TitleScreenManager Instance = null;

        private VisualElement titleScreenElement;
        private VisualElement optionsScreenElement;

        public new class UxmlFactory : UxmlFactory<TitleScreenManager, UxmlTraits> { }

        public TitleScreenManager()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new System.Exception("Cannot create more than one TitleScreenManager instance");
            }
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        void OnGeometryChange(GeometryChangedEvent evt)
        {
            titleScreenElement = this.Q("TitleScreen");
            optionsScreenElement = this.Q("OptionsScreen");

            titleScreenElement?.Q("create-new-workstation")?.RegisterCallback<ClickEvent>(e => CreateNewWorkstation());
            titleScreenElement?.Q("load-workstation")?.RegisterCallback<ClickEvent>(e => LoadWorkstation());
            titleScreenElement?.Q("options")?.RegisterCallback<ClickEvent>(e => OpenOptionsScreen());
            titleScreenElement?.Q("exit")?.RegisterCallback<ClickEvent>(e => Exit());

            optionsScreenElement?.Q("back-button")?.RegisterCallback<ClickEvent>(ev => OpenTitleScreen());

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        void OpenTitleScreen()
        {
            titleScreenElement.style.display = DisplayStyle.Flex;
            optionsScreenElement.style.display = DisplayStyle.None;
        }

        void OpenOptionsScreen()
        {
            titleScreenElement.style.display = DisplayStyle.None;
            optionsScreenElement.style.display = DisplayStyle.Flex;
        }

        void CreateNewWorkstation()
        {
            titleScreenElement.style.display = DisplayStyle.None;
            optionsScreenElement.style.display = DisplayStyle.None;

            // TODO
        }

        void LoadWorkstation()
        {
            titleScreenElement.style.display = DisplayStyle.None;
            optionsScreenElement.style.display = DisplayStyle.None;

            // TODO
        }

        void Exit()
        {
            if (Application.isEditor)
            {
                Debug.Log("Exited");
            }
            else
            {
                Application.Quit();
            }
        }
    }
}