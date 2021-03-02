using Newtonsoft.Json;
using System;
using UnityEngine;

namespace WorkstationDesigner.Workstation
{
    /// <summary>
    /// Represents the important data of a GameObject transform
    /// 
    /// This class is used to serialize data to a file and restore transform state from a file
    /// </summary>
    [Serializable]
    public class TransformData
    {
        [JsonProperty]
        public Vector3 Position { get; private set; }

        [JsonProperty]
        public Quaternion Rotation { get; private set; }

        [JsonProperty]
        public Vector3 LocalScale { get; private set; }

        /// <summary>
        /// Create a new TransformData object populated with state from a GameObject's Transform
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static TransformData FromTransform(Transform transform)
        {
            return new TransformData
            {
                Position = transform.position,
                Rotation = transform.rotation,
                LocalScale = transform.localScale
            };
        }

        /// <summary>
        /// Set a GameObject's Transform to the data held by this class
        /// </summary>
        /// <param name="transform"></param>
        public void SetTransform(Transform transform)
        {
            transform.position = Position;
            transform.rotation = Rotation;
            transform.localScale = LocalScale;
        }
    }
}
