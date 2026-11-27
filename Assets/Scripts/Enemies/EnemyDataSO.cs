using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    public string enemyName;
    public float maxHealth = 20f;
    public float moveSpeed = 2.5f;
    public float damage = 10f;
    public float attackCooldown = 0.5f;
    public float attackRange = 1.1f;
    public int xpReward = 5;

    [Header("Visual")]
    public Color color = Color.red;
    public float scale = 1f;

    [Header("Special Behavior")]
    public bool isKamikaze = false;
}
