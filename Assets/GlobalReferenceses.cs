using UnityEngine;

public class GlobalReferenceses : MonoBehaviour
{
    public static GlobalReferenceses Instance { get; private set; }

    [Header("Explosion Effects")]
    public GameObject grenadeExplosionEffect;

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
}
