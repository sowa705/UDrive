using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDrive
{
    internal class BinarySerializer : VehicleSerializer
    {
        BinaryWriter Writer;

        public SerializationMode Mode;

        public BinarySerializer(BinaryWriter writer,SerializationMode mode)
        {
            Writer=writer;
            Mode=mode;
        }

        public override void RunUpdate()
        {
            Writer.Write((int)Mode);
            if (Mode == 0)
            {
                return;
            }
            Writer.Write(Vehicle.Rigidbody.position.x);
            Writer.Write(Vehicle.Rigidbody.position.y);
            Writer.Write(Vehicle.Rigidbody.position.z);
            Writer.Write(Vehicle.Rigidbody.velocity.x);
            Writer.Write(Vehicle.Rigidbody.velocity.y);
            Writer.Write(Vehicle.Rigidbody.velocity.z);
            Writer.Write(Vehicle.Rigidbody.rotation.x);
            Writer.Write(Vehicle.Rigidbody.rotation.y);
            Writer.Write(Vehicle.Rigidbody.rotation.z);
            Writer.Write(Vehicle.Rigidbody.rotation.w);
            foreach (var item in Vehicle.InputParameters)
            {
                Writer.Write((int)item.Key);
                Writer.Write(item.Value);
            }
            Writer.Write(0);
            if (Mode == SerializationMode.Full)
            {
                foreach (var item in Vehicle.StatefulComponents)
                {
                    Writer.Write(item.Key);
                    item.Value.SerializeState(Writer);
                }
                Writer.Write(0);
            }
        }
    }
}
