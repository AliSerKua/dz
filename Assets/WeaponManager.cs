using UnityEngine;
using System.Collections.Generic;
using static AmmoBox;
using static Weapon;
using System;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("Weapon Slots")]
    public List<GameObject> weaponSlots;
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;
    public int totalShotGunAmmo = 0;


    [Header("Throwables")]
    public int grenades = 0;
    public float throwForce = 40f;
    public GameObject grenadePrefab;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0f;
    public float forceMultiplierLimit = 2f;


    private bool isReloading = false;
    

    public bool IsReloading() => isReloading;
    public void SetReloading(bool value) => isReloading = value;




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (weaponSlots.Count > 0)
            activeWeaponSlot = weaponSlots[0];
    }

    private void Update()
    {
        
        foreach (var slot in weaponSlots)
        {
            if (slot == null) continue;
            slot.SetActive(slot == activeWeaponSlot);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchActiveSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchActiveSlot(1);

       
        if (Input.GetKey(KeyCode.G))
        {
            forceMultiplier += Time.deltaTime * 1.5f; 
            forceMultiplier = Mathf.Clamp(forceMultiplier, 0f, forceMultiplierLimit);
        }

       
        if (Input.GetKeyUp(KeyCode.G))
        {
            if (grenades > 0)
            {
                ThrowLethal();
            }
            forceMultiplier = 0f; 
        }
    }


    public void PickupWeapon(GameObject pickedUpWeapon)
    {
        if (pickedUpWeapon == null || activeWeaponSlot == null) return;
        AddWeaponIntoActiveSlot(pickedUpWeapon);
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        // ��������� ������� � �������, ��� ����� ����� ������
        Vector3 newWeaponPosition = pickedupWeapon.transform.position;
        Quaternion newWeaponRotation = pickedupWeapon.transform.rotation;

        // ����������� ������ ������ � �� �� ������� � ����������
        DropCurrentWeapon(newWeaponPosition, newWeaponRotation);

        // ������������� ����� ������ � ����
        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();
        pickedupWeapon.transform.localPosition = weapon.spawnPosition;
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;

        // �������� Rigidbody � Collider
        DestroyImmediate(pickedupWeapon.GetComponent<Rigidbody>());
        DestroyImmediate(pickedupWeapon.GetComponent<Collider>());
    }

    private void DropCurrentWeapon(Vector3 dropPosition, Quaternion dropRotation)
    {
        if (activeWeaponSlot == null || activeWeaponSlot.transform.childCount == 0) return;

        GameObject weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;
        if (weaponToDrop == null) return;

        Weapon oldWeapon = weaponToDrop.GetComponent<Weapon>();
        if (oldWeapon != null)
        {
            oldWeapon.isActiveWeapon = false;
            oldWeapon.animator.enabled = false;
        }

        weaponToDrop.transform.SetParent(null);

        // ������������� ������� � �������
        weaponToDrop.transform.position = dropPosition;
        weaponToDrop.transform.rotation = dropRotation;

        // ��������� ��������� ���� �����������
        if (!weaponToDrop.TryGetComponent<Collider>(out Collider col))
        {
            col = weaponToDrop.AddComponent<BoxCollider>();
        }
        col.isTrigger = false;

        // ��������� rigidbody ���� �����������
        if (!weaponToDrop.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb = weaponToDrop.AddComponent<Rigidbody>();
        }

        rb.isKinematic = true;  // �� ����� ������/���������
        rb.useGravity = false;  // �� ����� ������������� � �����
    }


    public void PickupAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;

            case AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;

            case AmmoType.ShotGunAmmo:
                totalShotGunAmmo += ammo.ammoAmount;
                break;
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (IsReloading() || slotNumber < 0 || slotNumber >= weaponSlots.Count || weaponSlots[slotNumber] == null)
            return;

        // ����������� �������� ������
        if (activeWeaponSlot?.transform.childCount > 0)
        {
            Weapon current = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            if (current != null) current.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        // ��������� ������ ������
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            if (newWeapon != null) newWeapon.isActiveWeapon = true;
        }
    }

    public void DecreaseTotalAmmo(int bulletsToDecrease, WeaponModel model)
    {
        switch (model)
        {
            case WeaponModel.A47:
            case WeaponModel.M4:
            case WeaponModel.M107:
                totalRifleAmmo = Mathf.Max(0, totalRifleAmmo - bulletsToDecrease);
                break;

            case WeaponModel.P2:
                totalPistolAmmo = Mathf.Max(0, totalPistolAmmo - bulletsToDecrease);
                break;

            case WeaponModel.SPAS:
                totalShotGunAmmo = Mathf.Max(0, totalShotGunAmmo - bulletsToDecrease);
                break;
        }
    }

    public int CheckAmmoLeftFor(WeaponModel model)
    {
        return model switch
        {
            WeaponModel.A47 or WeaponModel.M4 or WeaponModel.M107 => totalRifleAmmo,
            WeaponModel.SPAS => totalShotGunAmmo,
            WeaponModel.P2 => totalPistolAmmo,
            _ => 0
        };
    }

    private void DestroyIfExists<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component != null)
            Destroy(component);
    }

    public void PickupThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
           {
            case Throwable.ThrowableType.Grenade:
                PickupGrenade();
                break;
        }
    }

    private void PickupGrenade()
    {
        grenades += 1;

        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Grenade);
    }

    private void ThrowLethal()
    {
        if (grenadePrefab == null)
        {
            Debug.LogError("Grenade prefab is missing!");
            return;
        }

        if (throwableSpawn == null)
        {
            Debug.LogError("Throwable spawn point is missing!");
            return;
        }

        if (Camera.main == null)
        {
            Debug.LogError("Main camera is missing!");
            return;
        }

        GameObject throwable = Instantiate(grenadePrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        if (throwable == null) return;

        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        }

        Throwable throwableComponent = throwable.GetComponent<Throwable>();
        if (throwableComponent != null)
        {
            throwableComponent.hasBeenThrow = true;
        }

        grenades -= 1;
        HUDManager.Instance?.UpdateThrowables(Throwable.ThrowableType.Grenade);
    }

}
