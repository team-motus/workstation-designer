using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.Jobs
{
    public static class JobStack
    {
        private static readonly List<Job> JobList = new List<Job>();

        public static Job PullJob()
        {
            if (JobList.Count == 0)
            {
                return null;
            }

            Job newestJob = JobList[JobList.Count - 1];
            JobList.RemoveAt(JobList.Count - 1);
            return newestJob;
        }

        public static void AddJob(Job job)
        {
            JobList.Add(job);
        }
    }
}
