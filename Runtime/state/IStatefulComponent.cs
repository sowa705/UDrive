using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IStatefulComponent
{
    object SerializeState();
    void DeserializeState(object state);
}