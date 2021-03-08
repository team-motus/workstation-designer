using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace WorkstationDesigner.Util
{
    public static class UIUtil
    {
        /// <summary>
        /// Find all children of a VisualElement with a given name
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<VisualElement> FindAllWithName(VisualElement parent, string name)
        {
            List<VisualElement> result = new List<VisualElement>();

            foreach (var i in parent.Children())
            {
                if (i.name == name)
                {
                    result.Add(i);
                }
                result.AddRange(FindAllWithName(i, name));
            }

            return result;
        }
    }
}
