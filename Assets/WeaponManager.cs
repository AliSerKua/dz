using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    [Tooltip("Список слотов для оружия (обязательно инициализируй в инспекторе!)")]
    public List<GameObject> weaponSlots;

    [Tooltip("Текущий активный слот")]
    public GameObject activeWeaponSlot;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (weaponSlots == null || weaponSlots.Count == 0)
        {
            Debug.LogError("Weapon slots not assigned!");
            return;
        }

        // Безопасное назначение первого слота
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
        if (weapon == null)
        {
            Debug.LogWarning("Picked up object has no Weapon script!");
            return;
        }

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

    public void SwitchActiveSlot(int slotNumber)
    {
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
}
