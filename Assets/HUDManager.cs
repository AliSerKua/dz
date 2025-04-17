using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        if (WeaponManager.Instance == null || WeaponManager.Instance.activeWeaponSlot == null)
            return;

        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot()?.GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.currentAmmo}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

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
        string ammoPrefabPath = string.Empty;

        switch (model)
        {
            case Weapon.WeaponModel.P2:
                ammoPrefabPath = "Pistol_Ammo";
                break;
            case Weapon.WeaponModel.A47:
                ammoPrefabPath = "Rifle_Ammo";
                break;

            case Weapon.WeaponModel.M4:
                ammoPrefabPath = "Rifle_Ammo";
                break;

            default:
               
                return emptySlot;
        }

        return LoadSpriteFromPrefab(ammoPrefabPath);
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        string weaponPrefabPath = string.Empty;

        switch (model)
        {
            case Weapon.WeaponModel.P2:
                weaponPrefabPath = "P2_Weapon";
                break;
            case Weapon.WeaponModel.A47:
                weaponPrefabPath = "A47_Weapon";
                break;

            case Weapon.WeaponModel.M4:
                weaponPrefabPath = "M4_Weapon";
                break;
            default:
                
                return emptySlot;
        }

        return LoadSpriteFromPrefab(weaponPrefabPath);
    }

    private Sprite LoadSpriteFromPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);

        if (prefab == null)
        {
            
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

        
        return emptySlot;
    }
}
