using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace WorkstationDesigner.Jobs
{
    /// <summary>
    /// The states for the worker's state machine when working on a TransportationJob.
    /// </summary>
    enum TransportationState
    {
        MovingToPickUp,
        PickingUp,
        MovingToDelivery,
        Delivering
    }

    /// <summary>
    /// The states for the worker's state machine when working on an AssemblyJob.
    /// </summary>
    enum AssemblyState
    {
        MovingToAssembly,
        Assembling
    }

    /// <summary>
    /// Controls the behavior of a single worker.
    /// </summary>
    public class WorkerController : MonoBehaviour
    {
        private Job CurrentJob;

        private SubstationInventory Inventory = new SubstationInventory(); // TODO: Should create subclass that can check worker constraints (e.g. worker can only carry so much)
        private TransportationState TransportationState;
        private AssemblyState AssemblyState;

        private NavMeshAgent NavMeshAgent;
        private TextMesh ActionDescription;

        /// <summary>
        /// Transforms a position in 3D space to an appropriate worker destination point.
        /// </summary>
        /// <param name="pos">The position to transform</param>
        /// <returns>The transformed position</returns>
        private Vector3 ToWorkerPos(Vector3 pos)
        {
            return new Vector3(pos.x, 0, pos.z);
        }

        /// <summary>
        /// Begins navigation of the worker to a destination.
        /// </summary>
        /// <param name="dest">The point to navigate to</param>
        private void StartMoveTo(Vector3 dest)
        {
            this.NavMeshAgent.SetDestination(ToWorkerPos(dest));
        }

        /// <summary>
        /// Begins navigation of the worker to the origin.
        /// </summary>
        private void MoveToOrigin()
        {
            StartMoveTo(new Vector3(0, 0, 0));
        }

        /// <summary>
        /// Determines whether the worker's current navigation has completed.
        /// </summary>
        /// <returns>Whether the worker's current navigation has completed</returns>
        private bool NavigationComplete()
        {
            return this.NavMeshAgent.isOnNavMesh && !this.NavMeshAgent.pathPending && (this.NavMeshAgent.remainingDistance <= this.NavMeshAgent.stoppingDistance) && (!this.NavMeshAgent.hasPath || this.NavMeshAgent.velocity.sqrMagnitude == 0f);
        }

        /// <summary>
        /// Coroutine corresponding to the MovingToPickup state of a TransportationJob.
        /// </summary>
        /// <returns>Required for coroutine definition</returns>
        private IEnumerator MoveToPickup()
        {
            TransportationJob job = (TransportationJob)CurrentJob;
            ActionDescription.text = "Moving to pick up: " + job.Element.ToString();
            this.TransportationState = TransportationState.MovingToPickUp;
            StartMoveTo(job.StartPos);
            while (!NavigationComplete()) { yield return null; }
            CompleteState();
        }

        /// <summary>
        /// Coroutine corresponding to the PickingUp state of a TransportationJob.
        /// </summary>
        /// <returns>Required for coroutine definition</returns>
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

        /// <summary>
        /// Coroutine corresponding to the MovingToDelivery state of a TransportationJob.
        /// </summary>
        /// <returns>Required for coroutine definition</returns>
        private IEnumerator MoveToDelivery()
        {
            TransportationJob job = (TransportationJob)CurrentJob;
            ActionDescription.text = "Moving to deliver: " + job.Element.ToString();
            this.TransportationState = TransportationState.MovingToDelivery;
            StartMoveTo(job.EndPos);
            while (!NavigationComplete()) { yield return null; }
            CompleteState();
        }

        /// <summary>
        /// Coroutine corresponding to the Delivering state of a TransportationJob.
        /// </summary>
        /// <returns>Required for coroutine definition</returns>
        private IEnumerator Deliver()
        {
            TransportationJob job = (TransportationJob)CurrentJob;
            ActionDescription.text = "Delivering: " + job.Element.ToString();
            this.TransportationState = TransportationState.Delivering;
            yield return new WaitForSeconds(2);
            job.DeliveryCallback();
            CompleteState();
        }

        /// <summary>
        /// Coroutine corresponding to the MovingToAssembly state of an AssemblyJob.
        /// </summary>
        /// <returns>Required for coroutine definition</returns>
        private IEnumerator MoveToAssembly()
        {
            AssemblyJob job = (AssemblyJob)CurrentJob;
            ActionDescription.text = "Moving to assembly: " + job.Description;
            this.AssemblyState = AssemblyState.MovingToAssembly;
            StartMoveTo(job.Position);
            while (!NavigationComplete()) { yield return null; }
            CompleteState();
        }

        /// <summary>
        /// Coroutine corresponding to the Assembling state of an AssemblyJob.
        /// </summary>
        /// <returns>Required for coroutine definition</returns>
        private IEnumerator Assemble()
        {
            AssemblyJob job = (AssemblyJob)CurrentJob;
            ActionDescription.text = "Assembling: " + job.Description;
            this.AssemblyState = AssemblyState.Assembling;
            yield return new WaitForSeconds(job.ExecutionTime);
            job.CompletionCallback();
            CompleteState();
        }

        /// <summary>
        /// Called to change states when a state has been completed.
        /// </summary>
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
            // If the worker doesn't have a job, try to get one from the JobStack
            if (CurrentJob == null)
            {
                CurrentJob = JobStack.PullJob();

                // Stop if there were no jobs for the JobStack to give
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
