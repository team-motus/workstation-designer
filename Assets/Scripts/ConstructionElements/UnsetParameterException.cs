using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorkstationDesigner.ConstructionElements
{
    [Serializable]
    public class UnsetParameterException : Exception
    {
        public UnsetParameterException() : base() { }
        public UnsetParameterException(string message) : base(message) { }
        public UnsetParameterException(string message, Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected UnsetParameterException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
