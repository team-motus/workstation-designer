using UnityEngine;
using WorkstationDesigner.Util;

namespace WorkstationDesigner.Workstation.Substations
{
    class PalletSubstation : PrefabSubstationBase
    {
        private const string PrefabPath = "Prefabs/MODS/Substations/Pallet";

        protected string ConstructionElementPrefabPath { get; private set; }
        protected bool ExtendConstructionElementLength { get; private set; }
        protected float ConstructionElementColorValueVariance { get; private set; }

        private const float StackHeight = 2f;

        public PalletSubstation(string name, int footprintLength1, int footprintLength2) : base(name, footprintLength1, footprintLength2, PrefabPath) { }

        public PalletSubstation(string name, int footprintLength1, int footprintLength2, 
            string constructionElementPrefabPath, bool extendConstructionElementLength = true, float constructionElementColorValueVariance = 0)
            : base(name, footprintLength1, footprintLength2, PrefabPath)
        {
            ConstructionElementPrefabPath = constructionElementPrefabPath;
            ExtendConstructionElementLength = extendConstructionElementLength;
            ConstructionElementColorValueVariance = constructionElementColorValueVariance;
        }

        private GameObject createElement(GameObject parent)
        {
            var gameObject = GameObject.Instantiate(Resources.Load<GameObject>(ConstructionElementPrefabPath));
            if (parent != null)
            {
                gameObject.transform.parent = parent.transform;
            }
            gameObject.transform.position = Vector3.zero;

            if (ConstructionElementColorValueVariance > 0)
            {
                // Slightly randomize color of construction elment
                var meshRenderer = gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    for (var i = 0; i < meshRenderer.materials.Length; i++)
                    {
                        meshRenderer.materials[i].color = (Random.Range(255 - 255 * ConstructionElementColorValueVariance, 255) / 255f) * meshRenderer.materials[i].color;
                    }
                }
            }

            return gameObject;
        }

        /// <summary>
        /// Calculate minimum position
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parentBounds"></param>
        /// <param name="elementBounds"></param>
        /// <param name="xCount"></param>
        /// <returns></returns>
        private Vector3 calculateMinPos(GameObject parent, Bounds parentBounds, Bounds elementBounds, int xCount)
        {
            // Calculate position of corner of pallet
            var minPos = SceneUtil.GetBottomPoint(parent) ?? Vector3.zero;
            minPos.x -= parentBounds.extents.x;
            minPos.y += parentBounds.size.y;
            // minPos.z -= bounds.Value.extents.z;

            // Offset by construction element extents
            minPos.x += elementBounds.extents.x;
            minPos.y += elementBounds.extents.y;

            // Pad x value to center stack on pallet
            minPos.x += 0.5f * (parentBounds.size.x - xCount * elementBounds.size.x);

            return minPos;
        }

        public override GameObject Instantiate()
        {
            // Create parent GameObject (the pallet)
            var parent = base.Instantiate();

            if (string.IsNullOrEmpty(ConstructionElementPrefabPath))
            {
                return parent;
            }

            // Get parent bounds
            var parentBounds = SceneUtil.GetBounds(parent);
            if (!parentBounds.HasValue)
            {
                Debug.LogError("PalletSubstation: Failed to determine parent bounds");
                return null;
            }

            // Get construction element bounds
            Bounds? elementBounds;
            {
                var gameObject = createElement(null);
                elementBounds = SceneUtil.GetBounds(gameObject);
                GameObject.Destroy(gameObject);

                if (!elementBounds.HasValue)
                {
                    Debug.LogError("PalletSubstation: Failed to determine construction element bounds");
                    return null;
                }
            }

            // Calculate number of construction elements to stack horizontally and vertically
            var xCount = Mathf.Max(1, (int)(parentBounds.Value.size.x / elementBounds.Value.size.x));
            var yCount = Mathf.Max(1, (int)(StackHeight / elementBounds.Value.size.y));

            var minPos = calculateMinPos(parent, parentBounds.Value, elementBounds.Value, xCount);

            // Scale length of construction element to match length of pallet
            var lengthScale = ExtendConstructionElementLength ? parentBounds.Value.size.z / elementBounds.Value.size.z : 1f;

            // Create stack of construction elements
            var pos = minPos;
            for (var y = 0; y < yCount; y++)
            {
                pos.x = minPos.x;
                for (var ix = 0; ix < xCount; ix++)
                {
                    var gameObject = createElement(parent);

                    gameObject.transform.localPosition = pos;

                    gameObject.transform.localScale = new Vector3(
                        gameObject.transform.localScale.x,
                        gameObject.transform.localScale.y * lengthScale,
                        gameObject.transform.localScale.z);

                    pos.x += elementBounds.Value.size.x;
                }
                pos.y += elementBounds.Value.size.y;
            }

            RecalculateCollider(parent);

            return parent;
        }
    }
}
