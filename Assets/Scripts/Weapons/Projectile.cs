using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float maxLifetime = 3f;

    private SpriteRenderer sr;
    private Vector2 direction;
    private float speed;
    private float damage;
    private float lifeTimer;
    private bool isReleased;
    private IObjectPool<Projectile> pool;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Launch(Vector3 startPos, Vector2 moveDir, float moveSpeed, float dmg, float lifetime, Color color, float scale, IObjectPool<Projectile> ownerPool)
    {
        transform.position = startPos;
        direction = moveDir.normalized;
        speed = moveSpeed;
        damage = dmg;
        sr.color = color;
        transform.localScale = Vector3.one * scale;
        pool = ownerPool;

        lifeTimer = lifetime;
        isReleased = false;
    }

    private void Update()
    {
        if (isReleased) return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isReleased) return;

        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
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