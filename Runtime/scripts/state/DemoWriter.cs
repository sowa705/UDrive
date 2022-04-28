using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DemoWriter : MonoBehaviour
{
    public UVehicle Target;
    public SerializationMode Mode;
    MemoryStream stream = new MemoryStream();
    BinaryWriter writer;
    // Start is called before the first frame update
    void Start()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ser();
    }
    void ser()
    {
        if (stream == null)
        {
            return;
        }
        if (Time.time > 60f)
        {
            Debug.Log("Serialization state written");
            stream.Flush();
            byte[] data = stream.ToArray();
            stream = null;
            File.WriteAllBytes("vehicledata.dat", data);
        }
        Target.SerializeState(writer, Mode);
    }
}
