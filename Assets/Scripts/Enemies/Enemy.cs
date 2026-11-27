using System;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    public event Action<float, float> OnHealthChanged;

    private EnemyDataSO data;
    private Transform target;
    private PlayerStats playerStats;
    private IObjectPool<Enemy> pool;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private float currentHealth;
    private float attackTimer;
    private bool isReleased;

    private float currentMaxHealth;
    private float currentDamage;
    private float currentMoveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void Initialize(
        EnemyDataSO enemyData,
        Transform playerTarget,
        IObjectPool<Enemy> ownerPool,
        float hpMult = 1f,
        float dmgMult = 1f,
        float speedMult = 1f)
    {
        data = enemyData;
        target = playerTarget;
        playerStats = playerTarget.GetComponent<PlayerStats>();
        pool = ownerPool;

        currentMaxHealth = data.maxHealth * hpMult;
        currentHealth = currentMaxHealth;
        currentDamage = data.damage * dmgMult;
        currentMoveSpeed = data.moveSpeed * speedMult;

        attackTimer = 0f;
        isReleased = false;

        sr.color = data.color;
        transform.localScale = Vector3.one * data.scale;

        OnHealthChanged?.Invoke(currentHealth, currentMaxHealth);
    }

    private void FixedUpdate()
    {
        if (target == null || isReleased) return;

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.linearVelocity = direction * currentMoveSpeed;
    }

    private void Update()
    {
        if (isReleased) return;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isReleased) return;

        if (collision.transform == target)
        {
            if (data.isKamikaze)
            {
                Explode();
                return;
            }

            if (attackTimer <= 0f)
            {
                playerStats?.TakeDamage(currentDamage);
                attackTimer = data.attackCooldown;
            }
        }
    }

    private void Explode()
    {
        if (isReleased) return;
        isReleased = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.attackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out PlayerStats player))
            {
                player.TakeDamage(currentDamage);
            }
            else if (hit.TryGetComponent(out Enemy otherEnemy) && otherEnemy != this)
            {
                otherEnemy.TakeDamage(currentDamage);
            }
        }

        pool.Release(this);
    }

    public void TakeDamage(float amount)
    {
        if (isReleased) return;

        currentHealth -= amount;
        OnHealthChanged?.Invoke(Mathf.Max(0f, currentHealth), currentMaxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isReleased) return;
        isReleased = true;

        EventBus.Instance.Broadcast(new EnemyDiedEvent(data.xpReward, transform.position));
        pool.Release(this);
    }
}