using UnityEngine;
using System.Collections.Generic;
using static AmmoBox;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    
    public static WeaponManager Instance { get; private set; }
    public List<GameObject> weaponSlots;
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;
    private bool isReloading = false;

    public bool IsReloading()
    {
        return isReloading;
    }

    public void SetReloading(bool value)
    {
        isReloading = value;
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    void Update()
    {
        // Безопасное переключение активного слота
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (weaponSlots[i] == null) continue;

            weaponSlots[i].SetActive(weaponSlots[i] == activeWeaponSlot);
        }

        // Переключение оружия по клавишам
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchActiveSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchActiveSlot(1);
    }

    public void PickupWeapon(GameObject pickedupWeapon)
    {
        if (pickedupWeapon == null || activeWeaponSlot == null) return;

        AddWeaponIntoActiveSlot(pickedupWeapon);
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon();

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();
      

        pickedupWeapon.transform.localPosition = weapon.spawnPosition;
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation);

        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;

        // Удаляем физику
        Rigidbody rb = pickedupWeapon.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        Collider col = pickedupWeapon.GetComponent<Collider>();
        if (col != null) Destroy(col);
    }

    private void DropCurrentWeapon()
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

        if (weaponToDrop.GetComponent<Rigidbody>() == null)
            weaponToDrop.AddComponent<Rigidbody>();

        if (weaponToDrop.GetComponent<Collider>() == null)
            weaponToDrop.AddComponent<BoxCollider>();

        Rigidbody rb = weaponToDrop.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(Camera.main.transform.forward * 3f + Vector3.up * 2f, ForceMode.Impulse);
    }

    internal void PickupAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;

            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (IsReloading())
            return;

        if (slotNumber < 0 || slotNumber >= weaponSlots.Count)
            return;

        if (weaponSlots[slotNumber] == null) return;

        // Деактивируем текущее оружие
        if (activeWeaponSlot != null && activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            if (currentWeapon != null) currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        // Активируем новое оружие
        if (activeWeaponSlot != null && activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            if (newWeapon != null) newWeapon.isActiveWeapon = true;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.A47:
                totalRifleAmmo -= bulletsToDecrease;
                break;

            case Weapon.WeaponModel.M4:
                totalRifleAmmo -= bulletsToDecrease;
                break;

            case Weapon.WeaponModel.P2:
                totalPistolAmmo -= bulletsToDecrease;
                break;
        }
    }

    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.A47:
                return totalRifleAmmo;

            case Weapon.WeaponModel.M4:
                return totalRifleAmmo;

            case Weapon.WeaponModel.P2:
                return totalPistolAmmo;

            default:
                return 0;
        }
    }
}
