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

            MapVisualElementChildren(parent, item =>
            {
                if (item.name == name)
                {
                    result.Add(item);
                }
            });

            return result;
        }

        public static void MapVisualElementChildren(VisualElement parent, Action<VisualElement> action)
        {
            foreach (var i in parent.Children())
            {
                action(i);
                MapVisualElementChildren(i, action);
            }
        }
    }
}
