using UnityEngine;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.Workstation.Substations
{
    public class PrefabSubstationBase : SubstationBase
    {
        protected string _PrefabPath { get; private set; }

        public PrefabSubstationBase(string name, int footprintLength1, int footprintLength2, string prefabPath) : base(name, footprintLength1, footprintLength2)
        {
            this._PrefabPath = prefabPath;
        }

        public override GameObject Instantiate()
        {
            var parent = new GameObject(this.GetType().Name);
            var gameObject = GameObject.Instantiate(Resources.Load<GameObject>(_PrefabPath));
            gameObject.transform.parent = parent.transform;

            var collider = parent.AddComponent<BoxCollider>();
            var bounds = SceneUtil.GetBounds(parent);

            if (bounds.HasValue)
            {
                collider.size = bounds.Value.size;
                collider.center = bounds.Value.center;
            }
            collider.isTrigger = true;

            var rigidBody = parent.AddComponent<Rigidbody>();
            rigidBody.isKinematic = false; // Attach a non-kinematic rigidbody to enable collision detection
            rigidBody.useGravity = false;

            return parent;
        }
    }
}
