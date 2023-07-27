using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseRigidbody : MonoBehaviour
{
    [Header("Rigidbody Properties")]
    [SerializeField] private float weightInKg = 2f;

    protected Rigidbody _rigidbody;
    protected float startDrag;
    protected float startAngularDrag;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (_rigidbody)
        {
            _rigidbody.mass = weightInKg;
            startDrag = _rigidbody.drag;
            startAngularDrag = _rigidbody.angularDrag;
        }
    }

    void FixedUpdate()
    {
        if (!_rigidbody)
        {
            return;
        }

        HandlePhysics();
    }

    protected virtual void HandlePhysics()
    {

    }
}
