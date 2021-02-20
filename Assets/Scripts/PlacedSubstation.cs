using UnityEngine;
using WorkstationDesigner.InputUtil;
using WorkstationDesigner.UI;

namespace WorkstationDesigner
{
    /// <summary>
    /// A component that has been placed in a workstation.
    /// </summary>
    public class PlacedSubstation : MonoBehaviour
    {
        public SubstationModel Substation { get; set; }

        // Update is called once per frame
        void Update()
        {
            if (MouseManager.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Make sure the raycast hit something
                if(Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        SubstationPlacementManager.Instance.MakePlacementSubstation(this.gameObject);
                    }
                }
            }
        }
    }
}
