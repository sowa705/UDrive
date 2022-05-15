using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    public abstract class VehicleComponent : MonoBehaviour
    {
        protected UVehicle vehicle;
        public UVehicle Vehicle { get => vehicle; }

        // Start is called before the first frame update
        void Start()
        {
            vehicle = GetComponentInParent<UVehicle>();
            if (vehicle == null)
            {
                Debug.LogError("Vehicle is null");
            }
            vehicle.AddVehicleComponent(this);
            VehicleStart();
        }
        /// <summary>
        /// Somewhat stable ID for this particular component. This value needs to be constant between game instances for the serialization system to work
        /// </summary>
        public int GetID()
        {
            if (vehicle == null)
            {
                vehicle = GetComponentInParent<UVehicle>();
            }
            if (vehicle == null)
            {
                return -1;
            }
            int cid = transform.localPosition.GetHashCode() ^ name.GetHashCode() ^ GetType().Name.GetHashCode();

            if (transform == vehicle.transform) // component attached to the vehicle root so we cant use the local position
            {
                cid = name.GetHashCode() ^ GetType().Name.GetHashCode();
            }
            //get a somewhat stable and consistent ID
            return cid;
        }
        /// <summary>
        /// Component start function. Use this instead of Unity Start()
        /// </summary>
        public virtual void VehicleStart()
        {

        }
    }
}