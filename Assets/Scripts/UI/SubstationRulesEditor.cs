using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Substations;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.UI
{
    public class SubstationRulesEditor : SidebarManager.ISidebar
    {
        private static VisualTreeAsset substationEditorAsset = null;
        private static VisualTreeAsset substationRuleAsset = null;
        private static VisualTreeAsset substationRuleIOAsset = null;

        private static Background? addIcon = null;
        private static Background? removeIcon = null;

        private VisualElement substationEditor;
        private VisualElement activeRulesListElement;
        private VisualElement allRulesListElement;

        private List<Rule> allRules;
        private List<Rule> activeRules;

        public class Rule
        {
            public string Type;
            public uint InputCount;
            public uint OutputCount;

            public Rule(string type, uint inputCount, uint outputCount)
            {
                this.Type = type;
                this.InputCount = inputCount;
                this.OutputCount = outputCount;
            }
        }

        public SubstationRulesEditor() : this(null) { }

        public SubstationRulesEditor(SubstationBase substation)
        {
            if (substationEditorAsset == null)
            {
                substationEditorAsset = Resources.Load<VisualTreeAsset>("UI/SubstationEditor");
            }
            if (substationRuleAsset == null)
            {
                substationRuleAsset = Resources.Load<VisualTreeAsset>("UI/SubstationRule");
            }
            if (substationRuleIOAsset == null)
            {
                substationRuleIOAsset = Resources.Load<VisualTreeAsset>("UI/substationRuleIO");
            }

            if (!addIcon.HasValue)
            {
                addIcon = Background.FromTexture2D(Resources.Load<Texture2D>("UI/images/add-icon-white"));
            }
            if (!removeIcon.HasValue)
            {
                removeIcon = Background.FromTexture2D(Resources.Load<Texture2D>("UI/images/close-icon-white"));
            }

            allRules = new List<Rule>();
            activeRules = new List<Rule>();

            // TODO test values
            allRules.Add(new Rule("Production", 2, 2));
            allRules.Add(new Rule("Production", 1, 1));
            allRules.Add(new Rule("Production", 0, 1));
            allRules.Add(new Rule("Production", 1, 1));
            activeRules.Add(new Rule("Production", 1, 1));

            allRulesListElement = MakeRuleListElement(allRules, false);
            activeRulesListElement = MakeRuleListElement(activeRules, true);

            substationEditor = substationEditorAsset.CloneTree().Q("substation-editor");

            var scrollView = substationEditor.Q<ScrollView>("scroll-view");

            scrollView.Q("active-rule-list").Add(activeRulesListElement);
            scrollView.Q("all-rule-list").Add(allRulesListElement);

            var saveButton = substationEditor.Q("save-button");
            saveButton.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == 0)
                {
                    Debug.Log("TODO Save");
                    SidebarManager.SetSidebar(new WorkstationRequirementsList());
                }
            });
        }

        private static VisualElement MakeRuleListElement(List<Rule> rules, bool active)
        {
            var list = new VisualElement();

            foreach (var rule in rules)
            {
                var ruleElement = substationRuleAsset.CloneTree().Q("substation-rule");

                // Set header to rule type
                var headerLabel = ruleElement.Q<Label>("header-label");
                headerLabel.text = $"{rule.Type}";

                // Setup rule button
                var ruleButton = ruleElement.Q("substation-rule-button");
                ruleButton.RegisterCallback<MouseDownEvent>(e =>
                {
                    if (e.button == 0)
                    {
                        Debug.Log("TODO Add / Remove");
                    }
                });

                var ruleButtonIcon = ruleButton.Q("substation-rule-button-icon");
                ruleButtonIcon.style.backgroundImage = active ? removeIcon.Value : addIcon.Value;

                // Set up inputs and outputs to rule
                var inputContainer = ruleElement.Q("substation-inputs");
                var outputContainer = ruleElement.Q("substation-outputs");

                if (rule.InputCount == 0)
                {
                    var ruleIOElement = CreateRuleIOElement("No Inputs");

                    var editButton = ruleIOElement.Q("substation-rule-io-edit-button");
                    editButton.RemoveFromHierarchy();

                    inputContainer.Add(ruleIOElement);
                }

                for (var i = 0; i < rule.InputCount; i++)
                {
                    var ruleIOElement = CreateRuleIOElement($"Input {i + 1}");

                    inputContainer.Add(ruleIOElement);
                }

                for (var i = 0; i < rule.OutputCount; i++)
                {
                    var ruleIOElement = CreateRuleIOElement($"Input {i + 1}");

                    outputContainer.Add(ruleIOElement);
                }

                // Hide all edit buttons if not active
                foreach (var editButton in UIUtil.FindAllWithName(ruleElement, "substation-rule-io-edit-button"))
                {
                    editButton.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
                }

                list.Add(ruleElement);
            }

            return list;
        }

        private static VisualElement CreateRuleIOElement(string labelText)
        {

            var ruleIOElement = substationRuleIOAsset.CloneTree().Q("substation-rule-io");

            var label = ruleIOElement.Q<Label>("substation-rule-io-label");
            label.text = labelText;

            var editButton = ruleIOElement.Q("substation-rule-io-edit-button");
            editButton.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == 0)
                {
                    Debug.Log("TODO Edit");
                }
            });

            return ruleIOElement;
        }

        public string GetHeaderText()
        {
            return "Substation Editor";
        }

        public VisualElement GetBody()
        {
            return this.substationEditor;
        }
    }
}
