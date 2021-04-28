using UnityEngine;

namespace WorkstationDesigner.Workstation.Substations
{
    /// <summary>
    /// An example substation that's simply a box the size of the substation footprint
    /// </summary>
    public class CubeSubstation : SubstationBase
    {
        public CubeSubstation(string name, int footprintLength1, int footprintLength2): base(name, footprintLength1, footprintLength2) { }

        public override GameObject Instantiate()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            gameObject.GetComponent<BoxCollider>().size = new Vector3((float)0.99, 1, (float)0.99);
            gameObject.GetComponent<BoxCollider>().isTrigger = true;

            var rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.isKinematic = false; // Attach a non-kinematic rigidbody to enable collision detection
            rigidBody.useGravity = false;

            gameObject.transform.localScale = new Vector3(this.FootprintDimensions.Item1, 4, this.FootprintDimensions.Item2);

            return gameObject;
        }
    }
}
