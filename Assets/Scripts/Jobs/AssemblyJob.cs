using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Jobs
{
    /// <summary>
    /// A job representing the assembly of some element(s) at a substation.
    /// </summary>
    public class AssemblyJob : Job
    {
        public Action CompletionCallback;
        public Vector3 Position;
        public int ExecutionTime;
        public String Description;
        // TODO: Animation

        /// <summary>
        /// Create a new assembly job.
        /// </summary>
        /// <param name="completionCallback">The callback to be run when assembly is completed</param>
        /// <param name="position">The position at which a worker should stand while assembling</param>
        /// <param name="executionTime">The amount of time in seconds the assembly takes to complete</param>
        /// <param name="description">A text description of the job</param>
        public AssemblyJob(Vector3 position, int executionTime, String description)
        {
            this.CompletionCallback = null;
            this.Position = position;
            this.ExecutionTime = executionTime;
            this.Description = description;
        }
    }
}
