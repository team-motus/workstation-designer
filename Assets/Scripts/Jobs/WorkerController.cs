using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Jobs
{
    enum TransportationState
    {
        MovingToPickUp,
        PickingUp,
        MovingToDelivery,
        Delivering
    }

    enum AssemblyState
    {
        MovingToAssembly,
        Assembling
    }

    public class WorkerController : MonoBehaviour
    {
        private Job CurrentJob;

        private SubstationInventory Inventory = new SubstationInventory(); // TODO: Rename/make subclass?
        private TransportationState TransportationState;
        private AssemblyState AssemblyState;

        private Vector3 ToWorkerPos(Vector3 pos)
        {
            return new Vector3(pos.x, 1, pos.z);
        }

        IEnumerator MoveToPickup()
        {
            Debug.Log("MOVING TO PICKUP");
            this.TransportationState = TransportationState.MovingToPickUp;
            TransportationJob job = (TransportationJob)CurrentJob;
            yield return new WaitForSeconds(1);
            this.transform.position = ToWorkerPos(job.StartPos);
            CompleteState();
        }

        IEnumerator PickUp()
        {
            Debug.Log("PICKING UP");
            this.TransportationState = TransportationState.PickingUp;
            TransportationJob job = (TransportationJob)CurrentJob;
            yield return new WaitForSeconds(1);
            Inventory.AddElements(job.Element, job.Quantity);
            job.PickupCallback();
            CompleteState();
        }

        IEnumerator MoveToDelivery()
        {
            Debug.Log("MOVING TO DELIVERY");
            this.TransportationState = TransportationState.MovingToDelivery;
            TransportationJob job = (TransportationJob)CurrentJob;
            yield return new WaitForSeconds(1);
            this.transform.position = ToWorkerPos(job.EndPos);
            CompleteState();
        }

        IEnumerator Deliver()
        {
            Debug.Log("DELIVERING");
            this.TransportationState = TransportationState.Delivering;
            TransportationJob job = (TransportationJob)CurrentJob;
            yield return new WaitForSeconds(1);
            job.DeliveryCallback();
            CompleteState();
        }

        IEnumerator MoveToAssembly()
        {
            Debug.Log("MOVING TO ASSEMBLY");
            this.AssemblyState = AssemblyState.MovingToAssembly;
            AssemblyJob job = (AssemblyJob)CurrentJob;
            yield return new WaitForSeconds(1);
            this.transform.position = ToWorkerPos(job.Position);
            CompleteState();
        }

        IEnumerator Assemble()
        {
            Debug.Log("ASSEMBLING");
            this.AssemblyState = AssemblyState.Assembling;
            AssemblyJob job = (AssemblyJob)CurrentJob;
            yield return new WaitForSeconds(1); // TODO: Should match set assembly time
            job.CompletionCallback();
            CompleteState();
        }

        private void CompleteState()
        {
            if (this.CurrentJob is TransportationJob)
            {
                switch (this.TransportationState)
                {
                    case TransportationState.MovingToPickUp:
                        StartCoroutine(PickUp());
                        break;
                    case TransportationState.PickingUp:
                        StartCoroutine(MoveToDelivery());
                        break;
                    case TransportationState.MovingToDelivery:
                        StartCoroutine(Deliver());
                        break;
                    case TransportationState.Delivering:
                        this.CurrentJob = null;
                        break;
                }
            }
            else
            {
                AssemblyJob job = (AssemblyJob)CurrentJob;
                switch (this.AssemblyState)
                {
                    case AssemblyState.MovingToAssembly:
                        StartCoroutine(Assemble());
                        break;
                    case AssemblyState.Assembling:
                        this.CurrentJob = null;
                        break;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            CurrentJob = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (CurrentJob == null)
            {
                CurrentJob = JobStack.PullJob();

                // Stop if there were no jobs for the job stack to give
                if (CurrentJob == null)
                {
                    return;
                }

                if (CurrentJob is TransportationJob)
                {
                    this.TransportationState = TransportationState.MovingToPickUp;
                    StartCoroutine(MoveToPickup());
                }
                else
                {
                    this.AssemblyState = AssemblyState.MovingToAssembly;
                    StartCoroutine(MoveToAssembly());
                }
            }

            
        }
    }
}
