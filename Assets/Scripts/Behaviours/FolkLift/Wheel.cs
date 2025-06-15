using UnityEngine;

namespace Behaviours.FolkLift
{
    [RequireComponent(typeof(WheelCollider))]
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private GameObject _mesh;

        private WheelCollider _wheelCollider;

        private void Awake()
        {
            _wheelCollider = GetComponent<WheelCollider>();
        }

        public void Refresh()
        {
            _wheelCollider.GetWorldPose(out var pos, out var rot);
            _mesh.transform.position = pos;
            _mesh.transform.rotation = rot;
        }

        public void SetTorque(float val)
        {
            _wheelCollider.motorTorque = val;
        }
        
        public void SetBrake(float val)
        {
            _wheelCollider.brakeTorque = val;
        }
        
        public void SetSteering(float val)
        {
            _wheelCollider.steerAngle = val;
        }
        
    }
}