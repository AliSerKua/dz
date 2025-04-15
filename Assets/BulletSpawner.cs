using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public AudioSource audioSource;
    public AudioClip shotSound;
    public ParticleSystem muzzleFlash;
    public float fireRate = 0.5f;

    private float nextFire = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFire)
        {
            nextFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("BulletPrefab или FirePoint не назначены!");
            return;
        }

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (audioSource && shotSound)
            audioSource.PlayOneShot(shotSound);

        if (muzzleFlash != null)
            muzzleFlash.Play();
    }
}
