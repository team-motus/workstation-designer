using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using WorkstationDesigner.Util;
using WorkstationDesigner.Workstation;

namespace WorkstationDesigner.UI
{
    public class TitleScreenManager : VisualElement
    {
        private VisualElement titleScreenElement;
        private VisualElement optionsScreenElement;

        public new class UxmlFactory : UxmlFactory<TitleScreenManager, UxmlTraits> { }

        public TitleScreenManager()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);   
        }

        void OnGeometryChange(GeometryChangedEvent evt)
        {
            titleScreenElement = this.Q("TitleScreen");
            optionsScreenElement = this.Q("OptionsScreen");

            titleScreenElement?.Q("create-new-workstation")?.RegisterCallback<ClickEvent>(e => CreateNewWorkstation());
            titleScreenElement?.Q("load-workstation")?.RegisterCallback<ClickEvent>(e => LoadWorkstation());
            titleScreenElement?.Q("options")?.RegisterCallback<ClickEvent>(e => OpenOptionsScreen());
            titleScreenElement?.Q("exit")?.RegisterCallback<ClickEvent>(e => AppUtil.Exit());

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
            
            AppUtil.LoadSceneAsync(AppUtil.MainSceneName, () => {
                UnityEngine.Debug.Log("TODO Create new workstation UI");
            });
        }

        void LoadWorkstation()
        {
            titleScreenElement.style.display = DisplayStyle.None;
            optionsScreenElement.style.display = DisplayStyle.None;

            AppUtil.LoadSceneAsync(AppUtil.MainSceneName, () => {
                WorkstationManager.PromptLoadWorkstation();
            });
        }
    }
}
