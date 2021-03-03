using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Util
{
    public class ResourceLoader: MonoBehaviour
    {
        private static Dictionary<string, object> loadedResources = new Dictionary<string, object>();

        private static Queue<(string, Type)> loadQueue = new Queue<(string, Type)>();

        public static void Load<TResource>(string resourcePath)
        {
            loadQueue.Enqueue((resourcePath, typeof(TResource)));
        }

        public static TResource Get<TResource>(string resourcePath)
        {
            return (TResource)loadedResources[resourcePath];
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
