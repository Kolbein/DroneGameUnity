using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunData _gunData;
    [SerializeField] private ParticleSystem _shootingSystem;
    [SerializeField] private ParticleSystem _impactParticalSystem;
    [SerializeField] private ParticleSystem _bloodImpactParticalSystem;
    [SerializeField] private Transform _camPosition;
    [SerializeField] private Transform _bulletSpawnPosition;
    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private float _bulletSpeed = 100f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClipArray;

    private DroneInputs _droneInputs;
    private Rigidbody _rigidbody;
    private float _timeSinceLastShot;

    private void Start()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
        _droneInputs = GetComponentInParent<DroneInputs>();
        _droneInputs.ShootAction += Shoot;
        _droneInputs.ReloadAction += StartReload;
        _gunData.CurrentAmmo = _gunData.MagSize;
        UIManager.Instance.AmmoText.text = $"{_gunData.CurrentAmmo} / {_gunData.MagSize}";
    }

    private void Update()
    {
        _timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(_camPosition.position, transform.forward * _gunData.MaxDistance);
    }

    private void OnDisable() => _gunData.IsReloading = false;

    private bool CanShoot() => !_gunData.IsReloading && _timeSinceLastShot > 1f / (_gunData.FireRate / 60f);

    private void Shoot()
    {
        if (_gunData.CurrentAmmo > 0)
        {
            if (CanShoot())
            {
                OnGunShot();
            }
        }
        else if (!_gunData.IsReloading)
        {
            StartReload();
        }
    }

    private void OnGunShot()
    {
        if (_shootingSystem != null)
            _shootingSystem.Play();

        if (Physics.Raycast(_camPosition.position, transform.forward, out RaycastHit hit, float.MaxValue))
        {
            IDamageable damageable = hit.transform.GetComponent<IDamageable>();
            damageable?.TakeDamage(_gunData.Damage);
            bool bloodOnImpact = damageable?.HasBlood() ?? false;

            TrailRenderer trail = Instantiate(_bulletTrail, _bulletSpawnPosition.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true, bloodOnImpact));

            AddForceToTarget(hit);
        }
        else
        {
            TrailRenderer trail = Instantiate(_bulletTrail, _bulletSpawnPosition.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, _bulletSpawnPosition.position + transform.forward * 100, Vector3.zero, false));
        }

        PlayShootingSound();
        AddRecoilForce();

        _gunData.CurrentAmmo--;
        _timeSinceLastShot = 0;

        UIManager.Instance.AmmoText.text = $"{_gunData.CurrentAmmo} / {_gunData.MagSize}";

        if (_gunData.CurrentAmmo == 0)
        {
            StartReload();
        }
    }

    private void AddForceToTarget(RaycastHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.GetComponent<Rigidbody>();
        if (hitRigidbody != null)
        {
            Vector3 forceDirection = hit.point - _bulletSpawnPosition.position;
            forceDirection.Normalize();
            hitRigidbody.AddForce(forceDirection * _gunData.Recoil);
        }
    }

    private void StartReload()
    {
        if (!_gunData.IsReloading && gameObject.activeSelf && _gunData.CurrentAmmo != _gunData.MagSize)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        _gunData.IsReloading = true;
        UIManager.Instance.ReloadingText.SetActive(true);

        yield return new WaitForSeconds(_gunData.ReloadTime);

        _gunData.CurrentAmmo = _gunData.MagSize;
        _gunData.IsReloading = false;
        UIManager.Instance.ReloadingText.SetActive(false);
        UIManager.Instance.AmmoText.text = $"{_gunData.CurrentAmmo} / {_gunData.MagSize}";
    }

    private void AddRecoilForce()
    {
        if (_rigidbody != null)
        {
            _rigidbody.AddForce(-_rigidbody.transform.forward * _gunData.Recoil);
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, bool madeImpact, bool bloodOnImpact = false)
    {
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= _bulletSpeed * Time.deltaTime;

            yield return null;
        }

        trail.transform.position = hitPoint;

        if (madeImpact && bloodOnImpact && _bloodImpactParticalSystem != null)
        {
            Instantiate(_bloodImpactParticalSystem, hitPoint, Quaternion.LookRotation(hitNormal));
        }
        else if (madeImpact && _impactParticalSystem != null)
        {
            Instantiate(_impactParticalSystem, hitPoint, Quaternion.LookRotation(hitNormal));
        }

        Destroy(trail.gameObject, trail.time);
    }

    private void PlayShootingSound()
    {
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(_audioClipArray[Random.Range(0, _audioClipArray.Length)], 0.5f);
        }
    }
}