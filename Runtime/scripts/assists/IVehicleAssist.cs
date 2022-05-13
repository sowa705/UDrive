using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDrive
{
    public interface IVehicleAssist
    {
        void OnWheel(UWheelCollider collider);
        void OnUpdate();
    }
}
