using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Jobs
{
    /// <summary>
    /// The list of jobs.
    /// Jobs are added to this list by substations and the TransportationManager.
    /// Workers pull jobs from the list to work on.
    /// </summary>
    public static class JobList
    {
        private static readonly List<Job> Jobs = new List<Job>();

        // TODO: Get job closest to worker
        /// <summary>
        /// Remove a job from the job list for a worker to work on.
        /// </summary>
        /// <returns>A job</returns>
        public static Job PullJob()
        {
            if (Jobs.Count == 0)
            {
                return null;
            }

            Job lastJob = Jobs[Jobs.Count - 1];
            Jobs.RemoveAt(Jobs.Count - 1);
            return lastJob;
        }

        /// <summary>
        /// Add a job to the job list.
        /// </summary>
        /// <param name="job">The job to add</param>
        public static void AddJob(Job job)
        {
            Jobs.Add(job);
        }
    }
}
