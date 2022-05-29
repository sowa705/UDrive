using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    /// <summary>
    /// Procedural suspension audio generator, requires AudioSource playing an empty clip placed BEFORE it on an object
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    public class SuspensionSound : VehicleComponent
    {
        UWheelCollider[] wheels;
        float[] substepbuf;
        int substep;

        float[] lastSus;
        double bufferbase;

        float samplerate;
        float fixeddt;

        [Range(0f, 1f)]
        public float Gain;
        // Start is called before the first frame update
        public override void VehicleStart()
        {
            wheels = vehicle.GetWheels();
            substepbuf = new float[vehicle.Substeps * 4];
            lastSus = new float[wheels.Length];

            samplerate = AudioSettings.outputSampleRate;
            fixeddt = Time.fixedDeltaTime;

            GetComponent<AudioLowPassFilter>().cutoffFrequency = 1f/Time.fixedDeltaTime;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            int dataLen = data.Length / channels;

            float dspdeltaT = dataLen / samplerate;

            float vehdeltaT = fixeddt / vehicle.Substeps;

            double delta = AudioSettings.dspTime - bufferbase;

            for (int i = 0; i < dataLen; i++)
            {
                double t = delta + (1f/samplerate) * i;

                int srcindex = (int)(t / vehdeltaT)%substepbuf.Length;

                data[i*channels] = substepbuf[srcindex]*Gain;
                data[i * channels+1] = substepbuf[srcindex] * Gain;

            }
        }

        public override void Substep()
        {
            substep++;

            if (substep>=vehicle.Substeps*4)
            {
                substep = 0;
                bufferbase = Time.fixedTimeAsDouble;
            }

            int index = substep;
            float delta = 0;
            for (int i = 0; i < wheels.Length; i++)
            {
                delta += lastSus[i] - wheels[i].LastTickState.SuspensionForce;
                lastSus[i] = wheels[i].LastTickState.SuspensionForce;
            }
            

            substepbuf[index] = delta/4000f;
        }
    }
}