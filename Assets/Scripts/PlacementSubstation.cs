using UnityEngine;
using UnityEngine.InputSystem;
using WorkstationDesigner.Substations;

namespace WorkstationDesigner
{
    /// <summary>
    /// A temporary version of a workstation component that is displayed when placing the component in the workspace.
    /// </summary>
    public class PlacementSubstation : MonoBehaviour
    {
        private const float ROTATE_SCALAR = 100;

        public SubstationBase Substation { get; set; }
        public bool IsIntersecting { get; private set; }

        private static Material IntersectionMaterial = null;

        public void Awake()
        {
            this.gameObject.layer = 2; // Ignore raycast

            if (IntersectionMaterial ==null)
            {
                IntersectionMaterial = Resources.Load<Material>("Materials/IntersectionMaterial");
            }
            this.GetComponent<Renderer>().sharedMaterial = IntersectionMaterial;
        }

        // Start is called before the first frame update
        public void Start()
        {
            this.IsIntersecting = false;
        }

        // Update is called once per frame
        public void Update()
        {
            Vector3? maybePlacePoint = SubstationPlacementManager.GetPlacementPoint();
            if (maybePlacePoint.HasValue)
            {
                Vector3 placePoint = maybePlacePoint.Value;
                placePoint.y += this.transform.localScale.y / 2;
                this.transform.position = placePoint;


                this.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                this.GetComponent<Renderer>().enabled = false;
            }

            if (Keyboard.current[Key.X].isPressed)
            {
                this.transform.Rotate(Vector3.up, ROTATE_SCALAR * Time.deltaTime, Space.World);
            }
            if (Keyboard.current[Key.C].isPressed)
            {
                this.transform.Rotate(Vector3.up, -ROTATE_SCALAR * Time.deltaTime, Space.World);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.GetComponent<PlacedSubstation>() != null)
            {
                this.IsIntersecting = true;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.GetComponent<PlacedSubstation>() != null)
            {
                this.IsIntersecting = false;
            }
        }
    }
}
