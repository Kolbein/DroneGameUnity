using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class DroneInputs : MonoBehaviour
{
    public Vector2 CyclicLeft { get; private set; }
    public Vector2 CyclicRight { get; private set; }
    public float Throttle { get; private set; }
    public bool IsHoverMode { get; private set; } = true;

    public Action ShootAction;
    public Action ReloadAction;

    private void OnCyclicLeft(InputValue value)
    {
        CyclicLeft = value.Get<Vector2>();
        Throttle = value.Get<Vector2>().y;
    }

    private void OnCyclicRight(InputValue value)
    {
        CyclicRight = value.Get<Vector2>();
    }

    private void OnHover()
    {
        IsHoverMode = !IsHoverMode;
    }

    private void OnShoot()
    {
        ShootAction?.Invoke();
    }

    private void OnReload()
    {
        ReloadAction?.Invoke();
    }
}
