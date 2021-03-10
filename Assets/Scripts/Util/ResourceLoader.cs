using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Util
{
    public class ResourceLoader: MonoBehaviour
    {
        private static Dictionary<string, object> loadedResources = new Dictionary<string, object>();

        private static Queue<(string, Type)> loadQueue = new Queue<(string, Type)>();

        /// <summary>
        /// Load a resource within the Assets/Resources folder (do not provide file extension)
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resourcePath"></param>
        public static void Load<TResource>(string resourcePath)
        {
            if (resourcePath.Contains("."))
            {
                Debug.LogWarning($"Attempting to load resource at path \"{resourcePath}\". This path should not contain a file extension");
            }
            loadQueue.Enqueue((resourcePath, typeof(TResource)));
        }

        /// <summary>
        /// Check if a resource has been loaded
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        public static bool HasLoaded(string resourcePath)
        {
            return loadedResources.ContainsKey(resourcePath);
        }

        /// <summary>
        /// Fetch a loaded resource
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        public static TResource Get<TResource>(string resourcePath)
        {
            if (!HasLoaded(resourcePath))
            {
                throw new Exception($"No resource laoded at path \"{resourcePath}\"");
            }
            try
            {
                return (TResource)loadedResources[resourcePath];
            }
            catch(Exception e)
            {
                throw new Exception($"Failed to load resource from path \"{resourcePath}\" with type {typeof(TResource).FullName}", e);
            }
        }

        private void LoadResources()
        {
            while (loadQueue.Count != 0)
            {
                var next = loadQueue.Dequeue();
                var newResource = Resources.Load(next.Item1, next.Item2);
                loadedResources.Add(next.Item1, newResource);
            }
        }

        public void Awake()
        {
            LoadResources();
        }

        public void Update()
        {
            LoadResources();
        }
    }
}
