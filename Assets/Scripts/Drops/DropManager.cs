using UnityEngine;
using UnityEngine.Pool;

public class DropManager : MonoBehaviour
{
    [SerializeField] private HealthPickup healthPickupPrefab;
    [Range(0f, 1f)]
    [SerializeField] private float hpDropChance = 0.15f;

    private ObjectPool<HealthPickup> pool;

    private void Awake()
    {
        pool = new ObjectPool<HealthPickup>(
            createFunc: () =>
            {
                HealthPickup pickup = Instantiate(healthPickupPrefab);
                pickup.SetPool(pool);
                return pickup;
            },
            actionOnGet:     p => p.gameObject.SetActive(true),
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: p => Destroy(p.gameObject),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize:         30
        );
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    private void OnEnemyDied(EnemyDiedEvent e)
    {
        if (healthPickupPrefab == null) return;

        if (Random.value <= hpDropChance)
        {
            HealthPickup pickup = pool.Get();
            pickup.SpawnAt(e.Position);
        }
    }
}