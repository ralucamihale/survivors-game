using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider2D))]
public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float healPercent = 0.25f;
    [SerializeField] private float lifetime = 15f;

    private IObjectPool<HealthPickup> pool;
    private float lifeTimer;
    private bool isReleased;

    public void SetPool(IObjectPool<HealthPickup> ownerPool) => pool = ownerPool;

    public void SpawnAt(Vector3 position)
    {
        transform.position = position;
        lifeTimer = lifetime;
        isReleased = false;
    }

    private void Update()
    {
        if (isReleased) return;

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReleased) return;

        if (other.TryGetComponent(out PlayerStats playerStats))
        {
            if (playerStats.CurrentHealth >= playerStats.MaxHealth) return;

            playerStats.HealPercent(healPercent);
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (isReleased) return;
        isReleased = true;
        pool.Release(this);
    }
}