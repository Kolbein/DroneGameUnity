using UnityEngine;

public interface IMotor
{
    void InitMotor();
    void UpdateMotor(Rigidbody rg, DroneInputs input);
}