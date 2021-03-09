using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Substations;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.UI
{
    /// <summary>
    /// Substation rules editor sidebar
    /// </summary>
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

        /// <summary>
        /// List of all possible rules
        /// </summary>
        private List<Rule> allRules;

        /// <summary>
        /// List of rules active on the substation
        /// </summary>
        private List<Rule> activeRules;

        /// <summary>
        /// Represents a substation rule - TODO: this is a placeholder for the backend implementation
        /// </summary>
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
            // Set up the various assets involved
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

            // Add lists to the scroll view
            var editorScrollView = substationEditor.Q<ScrollView>("scroll-view");
            editorScrollView.Q("active-rule-list").Add(activeRulesListElement);
            editorScrollView.Q("all-rule-list").Add(allRulesListElement);

            // Setup the save button
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

        /// <summary>
        /// Set up a VisualElement that displays a list of substation rules
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="active">True if the list is the active list</param>
        /// <returns></returns>
        private static VisualElement MakeRuleListElement(List<Rule> rules, bool active)
        {
            // Create an element to hold the list
            var list = new VisualElement();

            foreach (var rule in rules)
            {
                // Create a new substation rule element
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
                        if (active)
                        {
                            Debug.Log("TODO Remove");
                        }
                        else
                        {
                            Debug.Log("TODO Add");
                        }
                    }
                });

                // Set the rule button icon
                var ruleButtonIcon = ruleButton.Q("substation-rule-button-icon");
                ruleButtonIcon.style.backgroundImage = active ? removeIcon.Value : addIcon.Value;

                // Set up the list of inputs of the rule
                var inputContainer = ruleElement.Q("substation-inputs");

                // If there are no rules, then display an element saying that
                if (rule.InputCount == 0)
                {
                    var ruleIOElement = CreateRuleIOElement("No Inputs", null);

                    var editButton = ruleIOElement.Q("substation-rule-io-edit-button");
                    editButton.RemoveFromHierarchy();

                    inputContainer.Add(ruleIOElement);
                }

                for (var i = 0; i < rule.InputCount; i++)
                {
                    inputContainer.Add(CreateRuleIOElement($"Input {i + 1}", () => Debug.Log("TODO Edit")));
                }

                // Set up the list of outputs of the rule
                var outputContainer = ruleElement.Q("substation-outputs");
                for (var i = 0; i < rule.OutputCount; i++)
                {
                    outputContainer.Add(CreateRuleIOElement($"Output {i + 1}", () => Debug.Log("TODO Edit")));
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

        /// <summary>
        /// Create a new rule input/output element given its label
        /// </summary>
        /// <param name="labelText"></param>
        /// <returns></returns>
        private static VisualElement CreateRuleIOElement(string labelText, Action editCallback)
        {

            var ruleIOElement = substationRuleIOAsset.CloneTree().Q("substation-rule-io");

            var label = ruleIOElement.Q<Label>("substation-rule-io-label");
            label.text = labelText;

            var editButton = ruleIOElement.Q("substation-rule-io-edit-button");
            editButton.RegisterCallback<MouseDownEvent>(e =>
            {
                if (e.button == 0 && editCallback != null)
                {
                    editCallback();
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
