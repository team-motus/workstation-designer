using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

        private NavMeshAgent NavMeshAgent;
        private TextMesh ActionDescription;

        private Vector3 ToWorkerPos(Vector3 pos)
        {
            return new Vector3(pos.x, 0, pos.z);
        }

        private void StartMoveTo(Vector3 dest)
        {
            this.NavMeshAgent.SetDestination(ToWorkerPos(dest));
        }

        private void MoveToOrigin()
        {
            StartMoveTo(new Vector3(0, 0, 0));
        }

        private bool NavigationComplete()
        {
            return this.NavMeshAgent.isOnNavMesh && !this.NavMeshAgent.pathPending && (this.NavMeshAgent.remainingDistance <= this.NavMeshAgent.stoppingDistance) && (!this.NavMeshAgent.hasPath || this.NavMeshAgent.velocity.sqrMagnitude == 0f);
        }

        private IEnumerator MoveToPickup()
        {
            TransportationJob job = (TransportationJob)CurrentJob;
            ActionDescription.text = "Moving to pick up: " + job.Element.ToString();
            this.TransportationState = TransportationState.MovingToPickUp;
            StartMoveTo(job.StartPos);
            while (!NavigationComplete()) { yield return null; }
            CompleteState();
        }

        private IEnumerator PickUp()
        {
            TransportationJob job = (TransportationJob)CurrentJob;
            ActionDescription.text = "Picking up: " + job.Element.ToString();
            this.TransportationState = TransportationState.PickingUp;
            yield return new WaitForSeconds(2);
            Inventory.AddElements(job.Element, job.Quantity);
            job.PickupCallback();
            CompleteState();
        }

        private IEnumerator MoveToDelivery()
        {
            TransportationJob job = (TransportationJob)CurrentJob;
            ActionDescription.text = "Moving to deliver: " + job.Element.ToString();
            this.TransportationState = TransportationState.MovingToDelivery;
            StartMoveTo(job.EndPos);
            while (!NavigationComplete()) { yield return null; }
            CompleteState();
        }

        private IEnumerator Deliver()
        {
            TransportationJob job = (TransportationJob)CurrentJob;
            ActionDescription.text = "Delivering: " + job.Element.ToString();
            this.TransportationState = TransportationState.Delivering;
            yield return new WaitForSeconds(2);
            job.DeliveryCallback();
            CompleteState();
        }

        private IEnumerator MoveToAssembly()
        {
            AssemblyJob job = (AssemblyJob)CurrentJob;
            ActionDescription.text = "Moving to assembly: " + job.Description;
            this.AssemblyState = AssemblyState.MovingToAssembly;
            StartMoveTo(job.Position);
            while (!NavigationComplete()) { yield return null; }
            CompleteState();
        }

        private IEnumerator Assemble()
        {
            AssemblyJob job = (AssemblyJob)CurrentJob;
            ActionDescription.text = "Assembling: " + job.Description;
            this.AssemblyState = AssemblyState.Assembling;
            yield return new WaitForSeconds(job.ExecutionTime);
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
                        ActionDescription.text = "Idle";
                        MoveToOrigin();
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
                        ActionDescription.text = "Idle";
                        MoveToOrigin();
                        break;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            CurrentJob = null;
            this.NavMeshAgent = this.GetComponent<NavMeshAgent>();
            this.ActionDescription = this.GetComponentInChildren<TextMesh>();
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
