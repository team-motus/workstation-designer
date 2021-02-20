using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.Elements;

namespace WorkstationDesigner.Jobs
{
    public class TransportationJob : Job
    {
        public Action PickupCallback, DeliveryCallback;
        public Element Element;
        public int Quantity;
        public Vector3 StartPos, EndPos;

        public TransportationJob(Action pickupCallback, Action deliveryCallback, Element element, int quantity, Vector3 startPos, Vector3 endPos) : base()
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
