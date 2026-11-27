using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileWeapon", menuName = "Scriptable Objects/Projectile Weapon")]
public class ProjectileWeaponDataSO : ScriptableObject
{
    public string weaponName;
    public float baseDamage = 10f;
    public float baseCooldown = 0.7f;
    public float baseProjectileSpeed = 12f;
    public float projectileLifetime = 3f;

    [Header("Spread / Multishot")]
    public int projectileCount = 1;
    public float spreadAngle = 20f;

    [Header("Visual")]
    public Color projectileColor = Color.cyan;   
    public float projectileScale = 1f;
}
