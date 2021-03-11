using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Workstation.Substations;
using WorkstationDesigner.Util;
using System.Text.RegularExpressions;

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
        private static VisualTreeAsset editSubstationRuleDialogBodyAsset = null;
        private static VisualTreeAsset editSubstationRuleDialogItemAsset = null;

        private static Background? addIcon = null;
        private static Background? removeIcon = null;

        private VisualElement substationEditor;
        private VisualElement activeRulesListElement;
        private VisualElement allRulesListElement;

        private const string EDIT_RULE_DIALOG_KEY = "EditRule";

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
            public List<string> Inputs;
            public List<string> Outputs;

            public Rule(string type, uint inputCount, uint outputCount)
            {
                this.Type = type;
                this.Inputs = new List<string>();
                for(var i = 0; i < inputCount; i++)
                {
                    this.Inputs.Add($"Input {i + 1}");
                }
                this.Outputs = new List<string>();
                for (var i = 0; i < outputCount; i++)
                {
                    this.Outputs.Add($"Output {i + 1}");
                }
            }
        }

        /// <summary>
        /// Represents construction element properties - TODO: this is a placeholder for the backend implementation
        /// </summary>
        public class ConstructionElementProperties
        {
            public List<(string, double)> Properties;

            public ConstructionElementProperties()
            {
                Properties = new List<(string, double)>();
                for(var i = 0; i < 3; i++)
                {
                    Properties.Add(($"Property {i + 1}", 1.0));
                }
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
            if (editSubstationRuleDialogBodyAsset == null)
            {
                editSubstationRuleDialogBodyAsset = Resources.Load<VisualTreeAsset>("UI/EditSubstationRuleDialogBody");
            }
            if (editSubstationRuleDialogItemAsset == null)
            {
                editSubstationRuleDialogItemAsset = Resources.Load<VisualTreeAsset>("UI/EditSubstationRuleDialogItem");
            }
            CreateEditRuleDialog();

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
                if (rule.Inputs.Count == 0)
                {
                    var ruleIOElement = CreateRuleIOElement("No Inputs", null);

                    var editButton = ruleIOElement.Q("substation-rule-io-edit-button");
                    editButton.RemoveFromHierarchy();

                    inputContainer.Add(ruleIOElement);
                }

                for (var i = 0; i < rule.Inputs.Count; i++)
                {
                    var testProperties = new ConstructionElementProperties();
                    inputContainer.Add(CreateRuleIOElement(rule.Inputs[i], () => DialogToolkit.Open(EDIT_RULE_DIALOG_KEY, testProperties, e => CustomizeEditRuleDialog(e, testProperties))));
                }

                // Set up the list of outputs of the rule
                var outputContainer = ruleElement.Q("substation-outputs");
                for (var i = 0; i < rule.Outputs.Count; i++)
                {
                    var testProperties = new ConstructionElementProperties();
                    outputContainer.Add(CreateRuleIOElement(rule.Outputs[i], () => DialogToolkit.Open(EDIT_RULE_DIALOG_KEY, testProperties, e => CustomizeEditRuleDialog(e, testProperties))));
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

        /// <summary>
        /// Create dialog body for editing rules
        /// </summary>
        private void CreateEditRuleDialog()
        {
            if (!DialogToolkit.ContainsKey(EDIT_RULE_DIALOG_KEY))
            {
                VisualElement body = editSubstationRuleDialogBodyAsset.CloneTree();

                // Called on cancel
                Action<object> cancelCallback = obj =>
                {
                    var properties = obj as ConstructionElementProperties;
                    Debug.Log("TODO Cancel");
                };

                DialogToolkit.Create(
                    EDIT_RULE_DIALOG_KEY,
                    body,
                    cancelCallback,
                    new List<(string, Action<object>)>()
                    {
                        ("Cancel", obj => cancelCallback(obj)),
                        ("Save", obj => {
                            var properties = obj as ConstructionElementProperties;
                            if (properties == null)
                            {
                                Debug.LogError("Error properties is null");
                            }

                            // Fetch each textField in order
                            var textFields = new List<TextField>();
                            UIUtil.MapVisualElementChildren(body, item =>
                            {
                                if (item is TextField textField)
                                {
                                    textFields.Add(textField);
                                }
                            });

                            // Update values from textFields
                            for (var i = 0; i < properties.Properties.Count; i++)
                            {
                                properties.Properties[i] = (properties.Properties[i].Item1, double.Parse(textFields[i].value));
                            }

                            // Print results - TODO remove
                            var result = "";
                            for (var i = 0; i < properties.Properties.Count; i++)
                            {
                                result += $"({properties.Properties[i].Item1}, {properties.Properties[i].Item2})\n";
                            }
                            Debug.Log("TODO Save\n{" + result + "}");
                        })
                    }
                );
            }
        }

        private static void CustomizeEditRuleDialog(VisualElement element, ConstructionElementProperties properties)
        {
            var list = element.Q<ScrollView>();
            foreach (var i in properties.Properties)
            {
                var row = editSubstationRuleDialogItemAsset.CloneTree();
                var textField = row.Q<TextField>();

                // Set label and value from properties
                textField.label = i.Item1;
                textField.value = i.Item2.ToString();

                // Only allow users to enter doubles into text fields
                textField.RegisterCallback<ChangeEvent<string>>(e =>
                {
                    var orig = e.newValue;
                    var value = Regex.Replace(e.newValue, @"[^0-9.-]", ""); // Only allow integers, ".", and "-"
                    if (value.Length > 0)
                    {
                        var i = value.IndexOf('.');
                        if (i != -1)
                        {
                            value = value.Substring(0, i + 1) + value.Substring(i).Replace(".", ""); // Only allow one "."
                        }
                        i = value.IndexOf('-');
                        if (i != -1)
                        {
                            value = value.Substring(0, 1) + value.Substring(1).Replace("-", ""); // Only allow one "-" at the beginning
                        }
                    }
                    textField.SetValueWithoutNotify(value);
                });
                list.Add(row);
            }
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
