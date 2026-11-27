using UnityEngine;
using UnityEngine.Pool;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    private ObjectPool<Enemy> pool;

    public int ActiveEnemyCount => pool != null ? pool.CountActive : 0;

    private void Awake()
    {
        pool = new ObjectPool<Enemy>(
            createFunc:      () => Instantiate(enemyPrefab),
            actionOnGet:     e => e.gameObject.SetActive(true),
            actionOnRelease: e => e.gameObject.SetActive(false),
            actionOnDestroy: e => Destroy(e.gameObject),
            collectionCheck: true,
            defaultCapacity: 30,
            maxSize:         200
        );
    }

    public Enemy Create(EnemyDataSO data, Vector3 position, Transform playerTarget, float hpMult = 1f, float dmgMult = 1f, float speedMult = 1f)
    {
        Enemy enemy = pool.Get();
        enemy.transform.position = position;
        enemy.Initialize(data, playerTarget, pool, hpMult, dmgMult, speedMult);
        return enemy;
    }
}