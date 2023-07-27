using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DroneMotor : MonoBehaviour, IMotor
{
    [Header("Engine Properties")]
    [SerializeField] private float _maxPower = 40f;
    [SerializeField] private float _lerpSpeed = 2f;

    [Header("Propeller properties")]
    [SerializeField] private Transform _propeller;
    [SerializeField] private float _propellerRotationSpeed = 10f;

    private float _finalMotorForce;

    public void InitMotor()
    {
    }

    public void UpdateMotor(Rigidbody rb, DroneInputs input)
    {
        float motorForce = input.Throttle * _maxPower / 4;

        if (input.IsHoverMode)
        {
            // Opposite drone down force, per motor
            motorForce += rb.mass * Physics.gravity.magnitude / 4; 
        }

        _finalMotorForce = Mathf.Lerp(_finalMotorForce, motorForce, Time.deltaTime * _lerpSpeed);

        rb.AddForce(transform.up * _finalMotorForce, ForceMode.Force);

        HandlePropeller(motorForce);
    }

    public void HandlePropeller(float throttle)
    {
        if (!_propeller)
        {
            return;
        }

        _propeller.Rotate(Vector3.up, _propellerRotationSpeed * throttle);
    }
}
