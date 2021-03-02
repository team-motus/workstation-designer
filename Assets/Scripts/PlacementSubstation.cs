using UnityEngine;
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

        public void Awake()
        {
            this.gameObject.layer = 2; // Ignore raycast
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


                this.GetComponent<Renderer>().enabled = !this.IsIntersecting;
            }
            else
            {
                this.GetComponent<Renderer>().enabled = false;
            }

            if (Input.GetKey(KeyCode.X))
            {
                this.transform.Rotate(Vector3.up, ROTATE_SCALAR * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.C))
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
