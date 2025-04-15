using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; set; }

    public TextMeshProUGUI ammoDisplay;

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

    public void UpdateAmmo(int currentAmmo, int reserveAmmo)
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = currentAmmo + " / " + reserveAmmo;
        }
    }
}
