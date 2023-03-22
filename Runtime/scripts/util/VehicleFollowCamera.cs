using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UDrive
{
    [ExecuteInEditMode]
    public class VehicleFollowCamera : MonoBehaviour
    {
        public bool AutoSetTarget;
        public UVehicle Target;
        [Range(1f, 20f)]
        public float Distance = 10;
        [Range(0f, 6f)]
        public float Elevation = 3;
        [Range(0f, 1f)]
        public float AngleMultiplier = 0.25f;
        [Range(1f, 25f)]
        public float RotationSpeed = 10f;

        [Range(0f, 10f)]
        public float FreeLookSensitivity = 3f;

        public bool AvoidCollision;
        public bool LookBack;
        float FreeLookTimer;
        Vector2 FreeLookShift;
        void LateUpdate()
        {
            if (Target == null)
            {
                if (AutoSetTarget)
                {
                    Target = FindObjectOfType<UVehicle>();
                }
                else
                {
                    return;
                }
            }
            float actualRotSpeed = RotationSpeed;

            if (LookBack)
            {
                actualRotSpeed = 200f;
            }

            LookBack = Input.GetKey(KeyCode.Q);

            var targetTransform = Target.transform;

            Vector3 position = new Vector3(0, Elevation, -Distance);
            if (LookBack)
            {
                //position = new Vector3(0, Elevation, Distance);
                actualRotSpeed = 200f;
            }
            FreeLookTimer -= Time.deltaTime;
            if (FreeLookTimer<0)
            {
                FreeLookShift = Vector2.Lerp(FreeLookShift, Vector2.zero,Time.deltaTime*10f);
            }

            Vector2 frameShift = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

            if (frameShift.sqrMagnitude>0)
            {
                FreeLookTimer = 5f;

                FreeLookShift += frameShift* FreeLookSensitivity;
            }

            float rotationAngle = Mathf.Atan(Elevation / Distance) * Mathf.Rad2Deg;

            var targetrotation = Quaternion.LookRotation(targetTransform.forward, targetTransform.up) * Quaternion.Euler(rotationAngle * AngleMultiplier, 0, 0);

            if (LookBack)
                targetrotation = Quaternion.LookRotation(-targetTransform.forward, targetTransform.up) * Quaternion.Euler(rotationAngle * AngleMultiplier, 0, 0);

            targetrotation *= Quaternion.Euler(FreeLookShift.x, FreeLookShift.y,0);
            var smoothRotation = Quaternion.Slerp(transform.rotation, targetrotation, Time.unscaledDeltaTime * actualRotSpeed);

            var positionoffset = smoothRotation * position;
            var targetPos = targetTransform.position + positionoffset;
            if (AvoidCollision)
            {
                RaycastHit hit;
                Vector3 dir = (positionoffset).normalized;
                Ray r = new Ray(Target.transform.position, dir);
                float length = (positionoffset).magnitude + 1f;
                LayerMask mask = Physics.DefaultRaycastLayers & (~LayerMask.GetMask("IgnoreCameraRaycast"));

                Debug.DrawRay(r.origin, dir * length, Color.red);

                if (Physics.Raycast(r, out hit, length, mask))
                {
                    Debug.DrawLine(r.origin, hit.point, Color.green);
                    targetPos = hit.point - (r.direction);
                }
            }

            transform.position = targetPos;
            transform.rotation = smoothRotation;
        }
    }
}