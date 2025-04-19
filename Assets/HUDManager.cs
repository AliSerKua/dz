using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;
    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;
    public GameObject middleDot;

    private readonly Dictionary<Weapon.WeaponModel, string> ammoSprites = new()
    {
        { Weapon.WeaponModel.P2, "Pistol_Ammo" },
        { Weapon.WeaponModel.A47, "Rifle_Ammo" },
        { Weapon.WeaponModel.M107, "Rifle_Ammo" },
        { Weapon.WeaponModel.M4, "Rifle_Ammo" },
        { Weapon.WeaponModel.SPAS, "ShotGun_Ammo" }
    };

    private readonly Dictionary<Weapon.WeaponModel, string> weaponSprites = new()
    {
        { Weapon.WeaponModel.P2, "P2_Weapon" },
        { Weapon.WeaponModel.A47, "A47_Weapon" },
        { Weapon.WeaponModel.M107, "SniperRifle_Weapon" },
        { Weapon.WeaponModel.M4, "M4_Weapon" },
        { Weapon.WeaponModel.SPAS, "Shotgun_Weapon" }
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        var weaponManager = WeaponManager.Instance;
        if (weaponManager == null || weaponManager.activeWeaponSlot == null)
            return;

        Weapon activeWeapon = weaponManager.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot()?.GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.currentAmmo}";
            totalAmmoUI.text = $"{weaponManager.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;

            ammoTypeUI.sprite = GetAmmoSprite(model);
            activeWeaponUI.sprite = GetWeaponSprite(model);
        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";
            ammoTypeUI.sprite = emptySlot;
            activeWeaponUI.sprite = emptySlot;
        }

        if (unActiveWeapon)
        {
            unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
        }
        else
        {
            unActiveWeaponUI.sprite = emptySlot;
        }
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }
        return null;
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        if (!ammoSprites.TryGetValue(model, out var prefabPath))
            return emptySlot;

        return LoadSpriteFromPrefab(prefabPath);
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        if (!weaponSprites.TryGetValue(model, out var prefabPath))
            return emptySlot;

        return LoadSpriteFromPrefab(prefabPath);
    }

    private Sprite LoadSpriteFromPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);

        if (prefab == null)
        {
            Debug.LogWarning($"[HUDManager] Prefab '{prefabName}' not found in Resources.");
            return emptySlot;
        }

        Image image = prefab.GetComponent<Image>();
        if (image != null && image.sprite != null)
        {
            return image.sprite;
        }

        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            return sr.sprite;
        }

        Debug.LogWarning($"[HUDManager] No valid sprite found in prefab '{prefabName}'.");
        return emptySlot;
    }

    internal void UpdateThrowables(Throwable.ThrowableType throwable)
    {
        switch(throwable)
        {
            case Throwable.ThrowableType.Grenade:
                lethalAmountUI.text = $"{WeaponManager.Instance.grenades}";
                lethalUI.sprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }
}
