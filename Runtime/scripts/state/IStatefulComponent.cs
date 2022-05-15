using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDrive
{
    public interface IStatefulComponent
    {
        void SerializeState(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}