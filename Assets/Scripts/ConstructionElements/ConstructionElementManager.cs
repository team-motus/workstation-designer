using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorkstationDesigner.ConstructionElements.Elements;

namespace WorkstationDesigner.ConstructionElements
{
    public class ConstructionElementManager
    {
        private static ConstructionElementManager Instance = null;
        private List<ConstructionElement> ConstructionElements;

        public static ConstructionElementManager GetInstance()
        {
            if (ConstructionElementManager.Instance == null)
            {
                ConstructionElementManager.Instance = new ConstructionElementManager();
            }
            return ConstructionElementManager.Instance;
        }

        private ConstructionElementManager()
        {
            this.PopulateConstructionElements();
        }

        private void PopulateConstructionElements()
        {
            this.ConstructionElements = new List<ConstructionElement>()
            {
                new WoodPlank(),
                new Floor()
            };
        }

        public IEnumerable<ConstructionElement> GetUnits()
        {
            return this.ConstructionElements.Where(constructionElement => constructionElement.CanBeUnit);
        }
    }
}
