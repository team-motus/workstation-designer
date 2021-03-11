using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorkstationDesigner.ConstructionElements
{
    /// <summary>
    /// Class for an element - a transportable, requirable object that can be used in assembly.
    /// </summary>
    public abstract class ConstructionElement
    {
        public class Parameter
        {
            public string Name;
            public double? Value;

            public Parameter(string name)
            {
                this.Name = name;
                this.Value = null;
            }
        }

        public string Name;
        public List<Parameter> Parameters;
        public int MaxCarryable;
        public bool CanBeUnit;

        public ConstructionElement(string name, List<string> parameterNames, int maxCarryable, bool canBeUnit)
        {
            this.Name = name;
            this.Parameters = new List<Parameter>();
            foreach (string parameterName in parameterNames)
            {
                this.Parameters.Add(new Parameter(parameterName));
            }
            this.MaxCarryable = maxCarryable;
            this.CanBeUnit = canBeUnit;
        }

        protected Parameter FindParameter(string parameterName)
        {
            return this.Parameters.Find(parameter => parameter.Name == parameterName);
        }

        protected void SetParameterValue(string parameterName, double parameterValue)
        {
            FindParameter(parameterName).Value = parameterValue;
        }

        protected double GetParameterValue(string parameterName)
        {
            Parameter matchingParameter = FindParameter(parameterName);
            if (!matchingParameter.Value.HasValue)
            {
                throw new UnsetParameterException(parameterName);
            }
            return matchingParameter.Value.Value;
        }

        public override string ToString()
        {
            string displayString = this.Name;
            if (this.Parameters.Count > 0)
            {
                displayString += " (";
                for (int i = 0; i < this.Parameters.Count; i++)
                {
                    displayString += this.Parameters[i].Name + ": ";
                    if (this.Parameters[i].Value.HasValue)
                    {
                        displayString += this.Parameters[i].Value;
                    }
                    else
                    {
                        displayString += "X";
                    }

                    if (i < this.Parameters.Count - 1)
                    {
                        displayString += ", ";
                    }
                }
                displayString += ")";
            }
            return displayString;
        }
    }
}
