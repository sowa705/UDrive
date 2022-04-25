using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DemoReader : MonoBehaviour
{
    public UVehicle target;
    public string filename;
    Stream s;
    BinaryReader reader;
    // Start is called before the first frame update
    void Start()
    {
        s = new FileStream(filename,FileMode.Open, FileAccess.Read);
        reader = new BinaryReader(s);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target.DeserializeState(reader);
    }
}
