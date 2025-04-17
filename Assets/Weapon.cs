using UnityEngine;
using System.Collections;
using System;

public class Weapon : MonoBehaviour
{
    public enum WeaponModel
    {
        A47,
        M4,
        P2
    }

    public enum FireMode
    {
        Single,
        Burst,
        Auto
    }

    public FireMode fireMode = FireMode.Single;
    public int burstCount = 3; // для очереди
 
    public WeaponModel thisWeaponModel;

    public bool isActiveWeapon;
    public float damage = 21f;
    public float firerate = 1f;
    public float force = 155f;
    public int maxAmmo = 10;
    public int reserveAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;

    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    public ParticleSystem muzzleFlash;
    public Transform bulletSpawn;
    public AudioClip shotSFX, reloadSFX;
    public AudioSource audioSource;
    public Camera _cam;
    public GameObject hitEffect;

    private float nextFire = 0f;
    private bool isReloading = false;
    private Vector3 originalRotation;

    internal Animator animator;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    bool isADS;
    bool fireButtonReleased = true; // для одиночного режима

    void Start()
    {
        currentAmmo = maxAmmo;
        if (_cam != null)
            originalRotation = _cam.transform.localEulerAngles;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isActiveWeapon)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                SwitchFireMode();
            }

            if (Input.GetMouseButtonDown(1))
            {
                animator.SetTrigger("enterADS");
                isADS = true;
                HUDManager.Instance.middleDot.SetActive(false);
            }

            if (Input.GetMouseButtonUp(1))
            {
                animator.SetTrigger("exitADS");
                isADS = false;
                HUDManager.Instance.middleDot.SetActive(true);
            }

            GetComponent<Outline>().enabled = false;

            if (isReloading) return;

            HandleShooting();

            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0 && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                StartCoroutine(Reload());
            }

            if (_cam != null)
            {
                _cam.transform.localEulerAngles = Vector3.Lerp(_cam.transform.localEulerAngles, originalRotation, Time.deltaTime * 5f);
            }
        }
    }

    void HandleShooting()
    {
        switch (fireMode)
        {
            case FireMode.Single:
                if (Input.GetButtonDown("Fire1") && fireButtonReleased && Time.time >= nextFire && currentAmmo > 0)
                {
                    fireButtonReleased = false;
                    nextFire = Time.time + 1f / firerate;
                    Shoot();
                }
                if (Input.GetButtonUp("Fire1"))
                {
                    fireButtonReleased = true;
                }
                break;

            case FireMode.Auto:
                if (Input.GetButton("Fire1") && Time.time >= nextFire && currentAmmo > 0)
                {
                    nextFire = Time.time + 1f / firerate;
                    Shoot();
                }
                break;

            case FireMode.Burst:
                if (Input.GetButtonDown("Fire1") && currentAmmo >= burstCount && Time.time >= nextFire)
                {
                    StartCoroutine(BurstFire());
                    nextFire = Time.time + burstCount * (1f / firerate);
                }
                break;
        }
    }

    IEnumerator BurstFire()
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                yield return new WaitForSeconds(1f / firerate);
            }
        }
    }

    void SwitchFireMode()
    {
        fireMode = (FireMode)(((int)fireMode + 1) % Enum.GetValues(typeof(FireMode)).Length);
        Debug.Log("Fire mode switched to: " + fireMode.ToString());
    }

    void Shoot()
    {
        currentAmmo--;

        float spread = isADS ? adsSpreadIntensity : hipSpreadIntensity;

        Vector3 spreadDirection = bulletSpawn.forward +
                                  bulletSpawn.up * UnityEngine.Random.Range(-spread, spread) +
                                  bulletSpawn.right * UnityEngine.Random.Range(-spread, spread);
        spreadDirection.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(bulletSpawn.position, spreadDirection, out hit, 100f))
        {
            if (hitEffect != null)
            {
                GameObject impactGO = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1f);
            }
        }

        if (audioSource && shotSFX)
            audioSource.PlayOneShot(shotSFX);

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop();
            muzzleFlash.Play();
        }

        if (isADS)
            animator.SetTrigger("RECOIL_ADS");
        else
            animator.SetTrigger("RECOIL");

        GameObject bullet = BulletPool.Instance.GetBullet(bulletSpawn.position, Quaternion.LookRotation(spreadDirection));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(spreadDirection * force, ForceMode.Impulse);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        WeaponManager.Instance.SetReloading(true);

        if (audioSource && reloadSFX)
            audioSource.PlayOneShot(reloadSFX);

        if (animator)
            animator.SetTrigger("RELOAD");

        yield return new WaitForSeconds(reloadTime);

        int neededAmmo = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(neededAmmo, reserveAmmo);
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        int totalAmmoAvailable = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
        int bulletsLeft = Mathf.Min(ammoToReload, totalAmmoAvailable);
        WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);

        isReloading = false;
        WeaponManager.Instance.SetReloading(false);
    }
}
