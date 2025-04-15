using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public GameObject hitEffect;  // ������ ��� ���������

    private Rigidbody rb;
    private float timer;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        timer = 0f;

        // ��������� ���� ��� �������� ����
        if (rb != null)
        {
            rb.isKinematic = false;  // ������� ������ ������������
            rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);  // ���������� ����
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
        // ���������, ��� ������ �����
        if (hitEffect != null)
        {
            // ������� ������ �� ����� ���������
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // ������������ ���� ����� ���������
        DeactivateBullet();
    }

    private void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }
}