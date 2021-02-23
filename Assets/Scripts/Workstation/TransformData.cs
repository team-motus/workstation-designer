using Newtonsoft.Json;
using System;
using UnityEngine;

namespace WorkstationDesigner.Workstation
{
    [Serializable]
    public class TransformData
    {
        [JsonProperty]
        public Vector3 Position { get; private set; }

        [JsonProperty]
        public Quaternion Rotation { get; private set; }

        [JsonProperty]
        public Vector3 LocalScale { get; private set; }

        public static TransformData FromTransform(Transform transform)
        {
            return new TransformData
            {
                Position = transform.position,
                Rotation = transform.rotation,
                LocalScale = transform.localScale
            };
        }

        public void SetTransform(Transform transform)
        {
            transform.position = Position;
            transform.rotation = Rotation;
            transform.localScale = LocalScale;
        }
    }
}
