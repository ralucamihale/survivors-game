using System.Collections;
using UnityEngine;

public class AoEWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private float damage = 8f;
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visual")]
    [SerializeField] private GameObject visualCircle;
    [SerializeField] private float flashDuration = 0.2f;

    public float Cooldown => cooldown;

    private void Awake()
    {
        if (visualCircle != null)
        {
            visualCircle.SetActive(false);
        }
    }

    public void Fire(Transform target)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
        }

        if (visualCircle != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashVisual());
        }
    }

    private IEnumerator FlashVisual()
    {
        visualCircle.transform.localScale = Vector3.one * (radius * 2f);
        visualCircle.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        visualCircle.SetActive(false);
    }

    public void UpgradeRadius(float multiplier)   => radius *= multiplier;
    public void UpgradeFireRate(float cooldownMult) => cooldown *= cooldownMult;
}