using System;
using System.Collections.Generic;
using Abstractions;
using Enums;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours.FolkLift
{
    public class ForkLiftBase : MonoBehaviour, IForkLift
    {
        public ReadOnlyReactiveProperty<bool> IsEngineOn => _isEngineOn;
        public ReadOnlyReactiveProperty<TransmissionGearType> TransmissionGear => _transmissionGear;
        public ReadOnlyReactiveProperty<float> Fuel => _fuel;
        public float MaxFuel => _fuelMaxLevel;

        [SerializeField] ForksRails _forksRails;
        [SerializeField] private Wheel[] _steeringWheels, _drivingWheels;
        [SerializeField] private float  _maxSteering, _motorTorque, _brakeTorque;

        [SerializeField] private float _steeringVelocity, _steerReturnMaxVelocity;
        [SerializeField] private float _fuelConsumption;
        [SerializeField] private float _fuelMaxLevel;
        
        private readonly ReactiveProperty<TransmissionGearType> _transmissionGear = new(TransmissionGearType.N);
        private readonly ReactiveProperty<bool> _isEngineOn = new(false);
        private readonly ReactiveProperty<float> _acceleration = new();
        private readonly ReactiveProperty<float> _brakes = new();
        private readonly ReactiveProperty<float> _steering = new();
        private readonly ReactiveProperty<float> _fuel = new(0);

        private HashSet<Wheel> _wheels;
        
        private float _currentSteer;
        
        private Rigidbody _rb;

        private MainActions _input;

        private void Awake()
        {
            _fuel.Value = _fuelMaxLevel;
            _input = new MainActions();
            _input.ForkLift.Start.performed += ToggleEngine;
            _input.ForkLift.TransissionFirst.performed += TransmissionFirst;
            _input.ForkLift.TransissionRear.performed += TransmissionRear;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            
            _wheels = new HashSet<Wheel>();
            
            foreach (var steeringWheel in _steeringWheels)
            {
                _wheels.Add(steeringWheel);
            }

            foreach (var drivingWheel in _drivingWheels)
            {
                _wheels.Add(drivingWheel);
            }
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void OnDestroy()
        {
            _input.Dispose();
            _input.ForkLift.Start.performed -= ToggleEngine;
            _input.ForkLift.TransissionFirst.performed -= TransmissionFirst;
            _input.ForkLift.TransissionRear.performed -= TransmissionRear;
        }

        private void Update()
        {
            var moving = _input.ForkLift.Move.ReadValue<Vector2>();
            var ratio = GetRatioFromGear(_transmissionGear.CurrentValue);
            
            _steering.Value = -moving.x;
            _acceleration.Value = moving.y > 0 
                ? moving.y * ratio 
                : 0;
            _brakes.Value = moving.y < 0 ?  Mathf.Abs(moving.y) : 0;
            
            var lifting = _input.ForkLift.Lift.ReadValue<float>();
            _forksRails.SetLifting(lifting);
        }

        private void FixedUpdate()
        {
            if (_steering.CurrentValue == 0 && _currentSteer != 0)
            {
                var steerReturnVelocity = Mathf.Lerp(0, _steerReturnMaxVelocity, _rb.linearVelocity.magnitude / 5f);
                _currentSteer -= (_currentSteer / Mathf.Abs(_currentSteer)) * steerReturnVelocity * Time.fixedDeltaTime;
            }

            if (_steering.CurrentValue != 0)
            {
                _currentSteer = Mathf.Clamp(_currentSteer + _steering.CurrentValue * _steeringVelocity * Time.fixedDeltaTime , -1, 1);
            }
            
            if (_isEngineOn.CurrentValue)
            {
                _fuel.Value -= _fuelConsumption * Mathf.Abs(_acceleration.CurrentValue) * Time.fixedDeltaTime;

                var fuelModifier = _fuel.CurrentValue / _fuelMaxLevel < 0.5 ? 0.5f : 1f;
                
                foreach (var drivingWheel in _drivingWheels)
                {
                    drivingWheel.SetTorque(_acceleration.CurrentValue * fuelModifier * _motorTorque);
                }
            }
            
            foreach (var steeringWheel in _steeringWheels)
            {
                steeringWheel.SetSteering(_currentSteer * _maxSteering);
            }
            
            foreach (var wheel in _wheels)
            {
                wheel.SetBrake(_brakes.CurrentValue * _brakeTorque);
                wheel.Refresh();
            }
        }
        
        private void TransmissionFirst(InputAction.CallbackContext _)
        {
            _transmissionGear.Value = Enums.TransmissionGearType.M1;
        }
        
        private void TransmissionRear(InputAction.CallbackContext _)
        {
            _transmissionGear.Value = Enums.TransmissionGearType.R;
        }

        private void ToggleEngine(InputAction.CallbackContext _)
        {
            _isEngineOn.Value = !_isEngineOn.CurrentValue;
        }

        private float GetRatioFromGear(TransmissionGearType gearType) => gearType switch
        {
            Enums.TransmissionGearType.N => 0,
            Enums.TransmissionGearType.R => -1,
            Enums.TransmissionGearType.M1 => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(gearType), gearType, null)
        };
    }
}