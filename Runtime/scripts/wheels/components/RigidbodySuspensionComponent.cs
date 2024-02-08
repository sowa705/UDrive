using UnityEngine;

namespace UDrive
{
    public class RigidbodySuspensionComponent : WheelComponent
    {
        public Collider wheelColllider;
        public RigidbodySuspensionComponent(UWheelCollider collider) : base(collider)
        {
            var wheelObject = new GameObject("Wheel");
            
            wheelObject.transform.parent = collider.transform;
            wheelObject.transform.localPosition = Vector3.zero;
            wheelObject.transform.localRotation = Quaternion.identity;
            // mesh is 1x1x1, scale it to the wheel radius and width
            wheelObject.transform.localScale = new Vector3(collider.Parameters.Width, collider.Parameters.Radius * 2, collider.Parameters.Radius * 2);
            
            var meshFilter = wheelObject.AddComponent<MeshFilter>();
            meshFilter.mesh = collider.Parameters.ColliderMesh;
            /*
            var meshRenderer = wheelObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
            */
            var meshCollider = wheelObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.mesh;
            meshCollider.convex = true;
            meshCollider.isTrigger = true; // we don't want to collide with the wheel, we resolve collisions manually
            
            wheelColllider = meshCollider;
        }

        public override void RunSubstep(WheelTickState tickState, float deltaT)
        {
            wheelColllider.transform.position = Collider.GetWheelPosition();
            wheelColllider.transform.localRotation = Collider.GetWheelColliderLocalRotation();
            
            // overlap sphere to find nearby colliders
            Collider[] colliders = Physics.OverlapSphere(Collider.transform.position, Collider.Parameters.Radius+Collider.Parameters.Width, LayerMask.GetMask("Default"));
            
            Vector3 totalForce = Vector3.zero;

            float highestPenetration = 0;
            string highestPenetrationTag = "";
            var overlappedColliderCount = 0;
            // get the actual penetration depth of the wheel for each collider
            for (int i = 0; i < colliders.Length; i++)
            {
                Collider collider = colliders[i];
                // ignore vehicle body and any other colliders that are part of the vehicle
                if (collider.transform.IsChildOf(Collider.parentRB.transform))
                    continue;
                bool overlap = Physics.ComputePenetration(
                    wheelColllider,
                    wheelColllider.transform.position,
                    wheelColllider.transform.rotation,
                    collider,
                    collider.transform.position,
                    collider.transform.rotation,
                    out Vector3 direction,
                    out float distance);

                if (overlap)
                {
                    overlappedColliderCount++;
                    if (distance > highestPenetration)
                    {
                        highestPenetration = distance;
                        highestPenetrationTag = collider.tag;
                    }
                    Debug.DrawLine(Collider.transform.position, Collider.transform.position + direction * distance, Color.magenta, 1f);
                    totalForce += direction * (distance*5 / (deltaT * deltaT));
                }
            }
            // force that is not taken by the suspension and gets applied to the rigidbody directly
            Vector3 nonSuspensionForce = totalForce - Vector3.Dot(totalForce, Collider.transform.up) * Collider.transform.up;
            // apply the force to the rigidbody
            Collider.parentRB.AddForceAtPosition(nonSuspensionForce/ Collider.Vehicle.Substeps, Collider.GetWheelPosition(), ForceMode.Force);

            // calculate the force applied to the wheel in the suspension direction
            float collisionForce = Vector3.Dot(totalForce, Collider.transform.up);
            
            float springForce = 1 * Collider.Parameters.SuspensionSettings.Spring * Collider.wheelState.SuspensionPosition;
            float damperForce = 1 * Collider.wheelState.SuspensionVelocity * Collider.Parameters.SuspensionSettings.Damper;

            float force = springForce + damperForce;

            float maxForce = Collider.Vehicle.Rigidbody.mass * 10;

            force = Mathf.Clamp(force, -maxForce, maxForce);

            Vector3 vehicleBodyForce = force * Collider.transform.up;

            Collider.parentRB.AddForceAtPosition(vehicleBodyForce / Collider.Vehicle.Substeps, Collider.GetWheelPosition(), ForceMode.Force);

            Collider.wheelState.SuspensionVelocity -= ((force - collisionForce) * deltaT) / Collider.Parameters.Mass;
            Collider.wheelState.SuspensionVelocity = Mathf.Clamp(Collider.wheelState.SuspensionVelocity, -5, 5);
            Collider.wheelState.SuspensionPosition += Collider.wheelState.SuspensionVelocity * deltaT;
            Collider.wheelState.SuspensionPosition = Mathf.Clamp(Collider.wheelState.SuspensionPosition, -Collider.Parameters.SuspensionSettings.Travel * 1, Collider.Parameters.SuspensionSettings.Travel * 1);

            tickState.SuspensionForce = (force);
            tickState.TagName = highestPenetrationTag;
            tickState.IsGrounded = Mathf.Abs(collisionForce) > 1;
            tickState.ColliderCount = overlappedColliderCount;
        }

        public override void OnDetach()
        {
            Object.Destroy(wheelColllider.gameObject);
        }
    }
}