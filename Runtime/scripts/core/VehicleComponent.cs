using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleComponent : MonoBehaviour
{
    [NonSerialized]
    public int ID;
    protected UVehicle vehicle;
    public UVehicle Vehicle { get => vehicle; }
    // Start is called before the first frame update
    void Start()
    {
        vehicle = GetComponentInParent<UVehicle>();
        vehicle.AddVehicleComponent(this);
        VehicleStart();
    }

    public virtual void VehicleStart()
    {

    }
}
