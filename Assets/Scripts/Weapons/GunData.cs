using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{

    [Header("Info")]
    public string Name;

    [Header("Shooting")]
    public int Damage;
    public float MaxDistance;
    public float Recoil;

    [Header("Reloading")]
    public int CurrentAmmo;
    public int MagSize;
    [Tooltip("In RPM")] 
    public float FireRate;
    public float ReloadTime;
    [HideInInspector] 
    public bool IsReloading;

}