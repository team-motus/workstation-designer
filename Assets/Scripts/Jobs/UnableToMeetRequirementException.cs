using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner
{
    [Serializable]
    public class UnableToMeetRequirementException : Exception
    {
        public UnableToMeetRequirementException() : base() { }
        public UnableToMeetRequirementException(string message) : base(message) { }
        public UnableToMeetRequirementException(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected UnableToMeetRequirementException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
