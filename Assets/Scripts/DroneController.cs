using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DroneInputs))]
public class DroneController : BaseRigidbody
{
    [Header("Control Properties")]
    [SerializeField] private float _minMaxPitch = 30f;
    [SerializeField] private float _minMaxRoll = 30f;
    [SerializeField] private float _yawPower = 4f;
    [SerializeField] private float _lerpSpeed = 5f;

    private DroneInputs _input;
    private List<IMotor> _motors = new();

    private float _finalPitch;
    private float _finalRoll;
    private float _finalYaw;
    private float _yaw;

    void Start()
    {
        _input = GetComponent<DroneInputs>();
        _motors = GetComponentsInChildren<IMotor>().ToList();
    }


    protected override void HandlePhysics()
    {
        HandleEngines();
        HandleControls();
    }

    protected virtual void HandleEngines()
    {
        foreach (var motor in _motors)
        {
            motor.UpdateMotor(_rigidbody, _input);
        }
    }

    protected virtual void HandleControls()
    {
        float pitch = _input.CyclicRight.y * _minMaxPitch;      // Right stick up-down
        float roll = -_input.CyclicRight.x * _minMaxRoll;       // Right stick left-right
        _yaw += _input.CyclicLeft.x * _yawPower;                // Left stick left-right

        _finalPitch = Mathf.Lerp(_finalPitch, pitch, Time.deltaTime * _lerpSpeed);
        _finalRoll = Mathf.Lerp(_finalRoll, roll, Time.deltaTime * _lerpSpeed);
        _finalYaw = Mathf.Lerp(_finalYaw, _yaw, Time.deltaTime * _lerpSpeed);

        Quaternion orientation = Quaternion.Euler(_finalPitch, _finalYaw, _finalRoll);
        _rigidbody.MoveRotation(orientation);
    }
}
