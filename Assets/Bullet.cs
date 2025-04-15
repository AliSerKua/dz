using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public GameObject hitEffect;  // Эффект при попадании

    private Rigidbody rb;
    private float timer;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        timer = 0f;

        // Применяем силу для движения пули
        if (rb != null)
        {
            rb.isKinematic = false;  // Сделать объект динамическим
            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);  // Используем силу
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            DeactivateBullet();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Проверяем, что эффект задан
        if (hitEffect != null)
        {
            // Создаем эффект на месте попадания
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Деактивируем пулю после попадания
        DeactivateBullet();
    }

    private void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }
}