using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WorkstationDesigner.Substations;

namespace WorkstationDesigner.UI
{
    public class SubstationRulesEditor : SidebarManager.ISidebar
    {
        private static VisualTreeAsset substationRuleAsset = null;
        private const int ItemHeight = 180; // px // TODO

        private ListView activeRulesListView;
        private ListView allRulesListView;

        private List<Rule> allRules;
        private List<Rule> activeRules;

        public class Rule
        {
            public string Type;
            public string Input;
            public string Output;

            public Rule(string type)
            {
                this.Type = type;
            }
        }

        public SubstationRulesEditor() : this(null) { }

        public SubstationRulesEditor(SubstationBase substation)
        {
            if (substationRuleAsset == null)
            {
                substationRuleAsset = Resources.Load<VisualTreeAsset>("UI/SubstationRule");
            }

            allRules = new List<Rule>();
            activeRules = new List<Rule>();

            // TODO test values
            allRules.Add(new Rule("Production"));
            allRules.Add(new Rule("Production"));
            allRules.Add(new Rule("Production"));
            allRules.Add(new Rule("Production"));

            allRulesListView = MakeRuleListView(allRules);
            activeRulesListView = MakeRuleListView(activeRules);
        }

        private static ListView MakeRuleListView(List<Rule> rules)
        {
            Func<VisualElement> makeItem = () => substationRuleAsset.CloneTree();

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var label = e.Q<Label>("header-label");
                label.text = $"{rules[i].Type}";
            };

            var listView = new ListView(rules, ItemHeight, makeItem, bindItem);
            listView.selectionType = SelectionType.None;

            return listView;
        }

        public string GetHeaderText()
        {
            return "Substation Editor";
        }

        public VisualElement GetBody()
        {
            return this.allRulesListView;
        }
    }
}
