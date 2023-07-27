using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    private DroneInputs _input;

    private readonly float _smooth = 1f;
    private readonly float _tiltAngle = 10f;

    void Start()
    {
        _input = GetComponent<DroneInputs>();
    }

    void Update()
    {
        var positiveThrottle = _input.Throttle >= 0 ? _input.Throttle : 0;
        var positivePitch = _input.CyclicRight.y >= 0 ? _input.CyclicRight.y : 0;
        float throttlePitchProduct = positiveThrottle * positivePitch;

        // Rotate the drone by converting the angles into a quaternion.
        Quaternion target = Quaternion.Euler(throttlePitchProduct * -_tiltAngle, 0, 0);

        // Dampen towards the target rotation
        _camera.localRotation = Quaternion.Slerp(_camera.localRotation, target, Time.deltaTime * _smooth);
    }
}
