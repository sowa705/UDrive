using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UDrive
{
    public class TireSoundFX : MonoBehaviour
    {
        AudioSource source;
        public AudioClip clip;
        UWheelCollider wheel;
        float volume;
        // Start is called before the first frame update
        void Start()
        {
            source = CreateAndSetupSource();
            wheel = GetComponent<UWheelCollider>();
            source.clip = clip;
            source.Play();
        }
        AudioSource CreateAndSetupSource()
        {
            var source = gameObject.AddComponent<AudioSource>();

            source.loop = true;
            source.spatialize = true;
            source.spatialBlend = 1f;
            source.volume = 0;

            return source;
        }

        // Update is called once per frame
        void Update()
        {
            float totalForwardSlip = Mathf.Abs(wheel.debugData.SlipRatio - 1);
            float totalLateralSlip = Mathf.Abs(wheel.debugData.SlipAngle * 3f * (wheel.debugData.Velocity.y / 6f));

            if (!wheel.LastTickState.IsGrounded)
            {
                totalForwardSlip = 0;
                totalLateralSlip = 0;
            }

            volume = Mathf.Lerp(volume, (totalForwardSlip + totalLateralSlip) - 0.5f, Time.deltaTime * 2f);
            volume = Mathf.Clamp(volume, 0, 0.15f);
            source.volume = volume;
        }
    }
}
