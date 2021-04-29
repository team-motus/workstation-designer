using UnityEngine.UIElements;
using WorkstationDesigner.Util;
using WorkstationDesigner.Workstation;

namespace WorkstationDesigner.UI
{
    public class TitleScreenManager : VisualElement
    {
        private VisualElement titleScreenElement;

        public new class UxmlFactory : UxmlFactory<TitleScreenManager, UxmlTraits> { }

        public TitleScreenManager()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);   
        }

        void OnGeometryChange(GeometryChangedEvent evt)
        {
            titleScreenElement = this.Q("TitleScreen");

            titleScreenElement?.Q("create-new-workstation")?.RegisterCallback<ClickEvent>(e => CreateNewWorkstation());
            titleScreenElement?.Q("load-workstation")?.RegisterCallback<ClickEvent>(e => LoadWorkstation());
            titleScreenElement?.Q("exit")?.RegisterCallback<ClickEvent>(e => AppUtil.Exit());

            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        void CreateNewWorkstation()
        {
            titleScreenElement.style.display = DisplayStyle.None;
            
            AppUtil.LoadScene(AppUtil.MainSceneName, () => {
                WorkstationManager.New();
            });
        }

        void LoadWorkstation()
        {
            titleScreenElement.style.display = DisplayStyle.None;

            AppUtil.LoadScene(AppUtil.MainSceneName, () => {
                WorkstationManager.PromptOpen();
            });
        }
    }
}
