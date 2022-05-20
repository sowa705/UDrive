using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace UDrive
{
    public class DemoWriter : MonoBehaviour
    {
        public UVehicle Target;
        public SerializationMode Mode;
        MemoryStream stream;
        BinaryWriter writer;
        public string FileName;
        VehicleSerializer serializer;
        // Start is called before the first frame update
        void Start()
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
            serializer = new BinarySerializer(writer,Mode);
            Target.Serializer = serializer;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (stream == null)
            {
                return;
            }
            if (Time.time > 60f)
            {
                Target.Serializer = null;
                Debug.Log("Serialization state written");
                stream.Flush();
                byte[] data = stream.ToArray();
                stream = null;
                File.WriteAllBytes(FileName, data);
            }
        }

        private void OnDestroy()
        {
            if (stream!=null)
            {
                Target.Serializer = null;
                Debug.Log("Serialization state written");
                stream.Flush();
                byte[] data = stream.ToArray();
                stream = null;
                File.WriteAllBytes(FileName, data);
            }
        }
    }
}