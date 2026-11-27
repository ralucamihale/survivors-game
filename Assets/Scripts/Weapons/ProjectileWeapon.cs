using UnityEngine;
using UnityEngine.Pool;

public class ProjectileWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private ProjectileWeaponDataSO weaponData;
    [SerializeField] private Projectile projectilePrefab;

    private ObjectPool<Projectile> pool;

    private float currentDamage;
    private float currentCooldown;
    private float currentProjectileSpeed;
    private int currentProjectileCount;
    private float currentLifetime;

    public float Cooldown => currentCooldown;
    public ProjectileWeaponDataSO Data => weaponData;

    private void Awake()
    {
        ResetStats();

        pool = new ObjectPool<Projectile>(
            createFunc:      () => Instantiate(projectilePrefab),
            actionOnGet:     p => p.gameObject.SetActive(true),
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: p => Destroy(p.gameObject),
            collectionCheck: true,
            defaultCapacity: 30,
            maxSize:         150
        );
    }

    public void ResetStats()
    {
        currentDamage = weaponData.baseDamage;
        currentCooldown = weaponData.baseCooldown;
        currentProjectileSpeed = weaponData.baseProjectileSpeed;
        currentProjectileCount = weaponData.projectileCount;
        currentLifetime = weaponData.projectileLifetime;
    }

    public void Fire(Transform target)
    {
        if (target == null) return;

        Vector2 baseDir = (target.position - transform.position).normalized;
        float startAngle = -weaponData.spreadAngle * (currentProjectileCount - 1) / 2f;

        for (int i = 0; i < currentProjectileCount; i++)
        {
            float angle = startAngle + (weaponData.spreadAngle * i);
            Vector2 rotatedDir = (Vector2)(Quaternion.Euler(0f, 0f, angle) * baseDir);

            Projectile proj = pool.Get();
            proj.Launch(
                transform.position,
                rotatedDir,
                currentProjectileSpeed,
                currentDamage,
                weaponData.projectileLifetime,
                weaponData.projectileColor,
                weaponData.projectileScale,
                pool
            );
        }
    }

    public void UpgradeDamage(float multiplier) => currentDamage *= multiplier;
    public void UpgradeProjectileSpeed(float multiplier) => currentProjectileSpeed *= multiplier;
    public void UpgradeFireRate(float cooldownMultiplier) => currentCooldown *= cooldownMultiplier;
    public void AddProjectile(int amount = 1) => currentProjectileCount += amount;
}