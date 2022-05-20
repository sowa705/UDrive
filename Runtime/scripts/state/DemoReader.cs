using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UDrive
{
    public class DemoReader : MonoBehaviour
    {
        public UVehicle target;
        public string filename;
        Stream s;
        BinaryReader reader;
        VehicleDeserializer deserializer;

        // Start is called before the first frame update
        void Start()
        {
            s = new FileStream(filename, FileMode.Open, FileAccess.Read);
            reader = new BinaryReader(s);
            deserializer = new BinaryDeserializer(reader);
            target.Deserializer = deserializer;
        }
    }
}