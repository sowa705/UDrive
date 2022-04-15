using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleComponent : MonoBehaviour
{
    protected UVehicle vehicle;
    public UVehicle Vehicle { get => vehicle; }
    // Start is called before the first frame update
    void Start()
    {
        vehicle = GetComponentInParent<UVehicle>();
        VehicleStart();
    }

    public abstract void VehicleStart();
}
