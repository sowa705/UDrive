using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UDrive
{
    public class BinaryDeserializer : VehicleDeserializer
    {
        BinaryReader Reader;
        public BinaryDeserializer( BinaryReader reader )
        {
            Reader = reader;
        }

        public override void RunUpdate()
        {
            int m;
            try
            {
                m = Reader.ReadInt32();
            }
            catch (System.Exception)
            {
                throw new System.Exception("Cannot read from the stream");
            }
            if (m == 0)
            {
                throw new System.Exception("Invalid start header");
            }
            SerializationMode mode = (SerializationMode)m;
            if (m == 0)
            {
                return;
            }
            Vector3 pos = new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            Vector3 vel = new Vector3(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());
            Quaternion rot = new Quaternion(Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle(), Reader.ReadSingle());

            Debug.Log($"Position diff = {Vector3.Distance(pos, Vehicle.transform.position)}");
            Debug.Log($"Velocity diff = {Vector3.Distance(vel, Vehicle.Rigidbody.velocity)}");
            Debug.Log($"Rotation diff = {Quaternion.Angle(rot, Vehicle.Rigidbody.rotation)}");
            float dist = Vector3.Distance(pos, Vehicle.transform.position);
            if (dist > .5f)
            {
                Vehicle.Rigidbody.velocity = Vector3.Lerp(Vehicle.Rigidbody.velocity, vel, Time.fixedDeltaTime * dist);
                Vehicle.Rigidbody.position = Vector3.Lerp(Vehicle.Rigidbody.position, pos, Time.fixedDeltaTime * dist);
                Vehicle.Rigidbody.rotation = Quaternion.Slerp(Vehicle.Rigidbody.rotation, rot, Time.fixedDeltaTime * dist);
            }

            while (true)
            {
                VehicleInputParameter key = (VehicleInputParameter)Reader.ReadInt32();
                if (key == 0)
                {
                    break;
                }
                Vehicle.WriteInputParameter(key, Reader.ReadSingle());
            }
            if (mode == SerializationMode.Full)
            {
                while (true)
                {
                    int key = Reader.ReadInt32();
                    if (key == 0)
                    {
                        break;
                    }
                    if (!Vehicle.StatefulComponents.ContainsKey(key))
                    {
                        throw new System.Exception("Serialized stream contains unknown key");
                    }
                    Vehicle.StatefulComponents[key].Deserialize(Reader);
                }
            }
        }
    }
}
