using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }
    public Weapon hoveredWeapon = null;
    public AmmoBox hoveredAmmoBox = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            GameObject objectHit = hit.transform.gameObject;

            // Взаимодействие с оружием
            Weapon weapon = objectHit.GetComponent<Weapon>();
            if (weapon && !weapon.isActiveWeapon)
            {
                if (hoveredWeapon != weapon)
                {
                    if (hoveredWeapon)
                        hoveredWeapon.GetComponent<Outline>().enabled = false;

                    hoveredWeapon = weapon;
                    hoveredWeapon.GetComponent<Outline>().enabled = true;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupWeapon(objectHit);
                }
            }
            else
            {
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                    hoveredWeapon = null;
                }
            }

            // Взаимодействие с ящиком патронов
            AmmoBox ammoBox = objectHit.GetComponent<AmmoBox>();
            if (ammoBox)
            {
                if (hoveredAmmoBox != ammoBox)
                {
                    if (hoveredAmmoBox)
                        hoveredAmmoBox.GetComponent<Outline>().enabled = false;

                    hoveredAmmoBox = ammoBox;
                    hoveredAmmoBox.GetComponent<Outline>().enabled = true;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);
                    Destroy(objectHit.gameObject);
                }
            }
            else
            {
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                    hoveredAmmoBox = null;
                }
            }
        }
        else
        {
            
            if (hoveredWeapon)
            {
                hoveredWeapon.GetComponent<Outline>().enabled = false;
                hoveredWeapon = null;
            }

            if (hoveredAmmoBox)
            {
                hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                hoveredAmmoBox = null;
            }
        }
    }
}
        