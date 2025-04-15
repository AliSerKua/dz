using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public float damage = 21f;               // Урон пули
    public float firerate = 1f;              // Частота стрельбы
    public float force = 155f;               // Сила отдачи
    public int maxAmmo = 10;                 // Максимальное количество патронов в магазине
    public int reserveAmmo = 30;             // Количество патронов в запасе
    public int currentAmmo;                 // Текущее количество патронов в магазине
    public float reloadTime = 2f;            // Время перезарядки

    public ParticleSystem muzzleFlash;       // Вспышка при выстреле
    public Transform bulletSpawn;            // Точка спауна пули
    public AudioClip shotSFX, reloadSFX;     // Звуки выстрела и перезарядки
    public AudioSource audioSource;          // Источник звука
    public Camera _cam;                      // Камера для эффекта отдачи

    private float nextFire = 0f;             // Время следующего выстрела
    private bool isReloading = false;        // Флаг перезарядки
    private Vector3 originalRotation;        // Оригинальная позиция камеры для отдачи

    internal Animator animator;               // Аниматор для отдачи

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    void Start()
    {
        currentAmmo = maxAmmo;
        if (_cam != null)
            originalRotation = _cam.transform.localEulerAngles;
        animator = GetComponent<Animator>();
        AmmoManager.Instance?.UpdateAmmo(currentAmmo, reserveAmmo);
    }

    void Update()
    {
        if (isActiveWeapon)
        {

            if (isActiveWeapon)
            {
                GetComponent<Outline>().enabled = false;
            }

            if (isReloading) return;

            // Стрельба
            if (Input.GetButton("Fire1") && Time.time >= nextFire && currentAmmo > 0)
            {
                nextFire = Time.time + 1f / firerate;
                Shoot();
            }

            // Перезарядка
            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
            {
                StartCoroutine(Reload());
            }

            // Эффект отдачи камеры
            if (_cam != null)
            {
                _cam.transform.localEulerAngles = Vector3.Lerp(_cam.transform.localEulerAngles, originalRotation, Time.deltaTime * 5f);
            } 
        }
    }

    void Shoot()
    {
        currentAmmo--;  // Уменьшаем количество патронов

        Debug.Log("🟢 ВЫСТРЕЛ: Ammo = " + currentAmmo);

        // Звуки
        if (audioSource && shotSFX)
            audioSource.PlayOneShot(shotSFX);

        // Вспышка
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop();
            muzzleFlash.Play();
        }

        // Отдача
        if (animator)
            animator.SetTrigger("RECOIL");

        // Эффект отдачи для камеры
        if (_cam != null)
            _cam.transform.localEulerAngles += new Vector3(-5f, 0, 0);  // Добавляем небольшой угол

        // Получаем пулю из пула
        GameObject bullet = BulletPool.Instance.GetBullet(bulletSpawn.position, bulletSpawn.rotation);

        // Применяем силу на пулю
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Сбрасываем скорость перед применением новой
            rb.AddForce(bulletSpawn.forward * force, ForceMode.Impulse); // Придаем пуле скорость
        }

        // Обновляем количество патронов
        AmmoManager.Instance?.UpdateAmmo(currentAmmo, reserveAmmo);
    }

    IEnumerator Reload()
    {
        isReloading = true;

        // Звуки
        if (audioSource && reloadSFX)
            audioSource.PlayOneShot(reloadSFX);

        // Перезарядка анимации
        if (animator)
            animator.SetTrigger("RELOAD");

        yield return new WaitForSeconds(reloadTime);  // Ждем время перезарядки

        int neededAmmo = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(neededAmmo, reserveAmmo);  // Сколько патронов можно зарядить
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        AmmoManager.Instance?.UpdateAmmo(currentAmmo, reserveAmmo);
    }
}
