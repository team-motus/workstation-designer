using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Jobs
{
    public class AssemblyJob : Job
    {
        public Action CompletionCallback;
        public Vector3 Position;
        public int ExecutionTime;
        // TODO: Animation

        public AssemblyJob(Action completionCallback, Vector3 position, int executionTime)
        {
            this.CompletionCallback = completionCallback;
            this.Position = position;
            this.ExecutionTime = executionTime;
        }
    }
}
