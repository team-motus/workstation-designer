using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;

namespace WorkstationDesigner.Jobs
{
    /// <summary>
    /// A job representing the transportation of elements between two substations.
    /// </summary>
    public class TransportationJob : Job
    {
        public Action PickupCallback, DeliveryCallback;
        public ConstructionElement Element;
        public int Quantity;
        public Vector3 StartPos, EndPos;

        /// <summary>
        /// Create a new TransportationJob.
        /// </summary>
        /// <param name="pickupCallback">The callback to be run upon pickup</param>
        /// <param name="deliveryCallback">The callback to be run upon delivery</param>
        /// <param name="element">The element to be transported</param>
        /// <param name="quantity">The quantity of the element to be transported</param>
        /// <param name="startPos">The position the worker will stand at during pickup</param>
        /// <param name="endPos">The position the worker will stand at during delivery</param>
        public TransportationJob(Action pickupCallback, Action deliveryCallback, ConstructionElement element, int quantity, Vector3 startPos, Vector3 endPos) : base()
        {
            this.PickupCallback = pickupCallback;
            this.DeliveryCallback = deliveryCallback;
            this.Element = element;
            this.Quantity = quantity;
            this.StartPos = startPos;
            this.EndPos = endPos;
        }
    }
}
