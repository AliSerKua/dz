using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;
    [SerializeField] private AudioClip explosionSound; // Звук взрыва

    float countdown;
    bool hasExploaded = false;
    public bool hasBeenThrow = false;

    public enum ThrowableType
    {
        Grenade
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrow)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && !hasExploaded)
            {
                Exploade();
                hasExploaded = true;
            }
        }
    }

    private void Exploade()
    {
        GetThrowableEffect();
        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
        }
    }

    private void GrenadeEffect()
    {
        // Визуальный эффект
        GameObject explosionEffect = GlobalReferenceses.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Звук взрыва
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Физический эффект
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }
        }

        // Здесь можно добавить урон врагам
    }
}
