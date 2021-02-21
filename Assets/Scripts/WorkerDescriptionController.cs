using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    public class WorkerDescriptionController : MonoBehaviour
    {
        void Start()
        {
        
        }

        void Update()
        {
            // Look at the camera while maintaining a flip (so that the text faces the right direction)
            this.transform.rotation = Quaternion.LookRotation(this.transform.position - Camera.main.transform.position);
        }
    }
}
