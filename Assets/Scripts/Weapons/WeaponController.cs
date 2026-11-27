using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private float targetRange = 10f;
    [SerializeField] private float targetSearchInterval = 0.2f;
    [SerializeField] private LayerMask enemyLayer;

    private class WeaponSlot
    {
        public IWeapon weapon;
        public float timer;
    }

    private readonly List<WeaponSlot> equippedWeapons = new();
    private int activeWeaponIndex = 0;
    private Transform currentTarget;
    private float searchTimer;

    private void Awake()
    {
        foreach (IWeapon weapon in GetComponentsInChildren<IWeapon>())
        {
            AddWeapon(weapon);
        }
    }

    public void AddWeapon(IWeapon weapon)
    {
        equippedWeapons.Add(new WeaponSlot { weapon = weapon, timer = 0f });
    }

    private void Update()
    {
        HandleWeaponSwitch();

        if (equippedWeapons.Count == 0) return;

        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0f)
        {
            searchTimer = targetSearchInterval;
            currentTarget = FindNearestEnemy();
        }

        foreach (WeaponSlot slot in equippedWeapons)
        {
            if (slot.timer > 0f)
                slot.timer -= Time.deltaTime;
        }

        WeaponSlot activeSlot = equippedWeapons[activeWeaponIndex];
        if (activeSlot.timer <= 0f)
        {
            activeSlot.weapon.Fire(currentTarget);
            activeSlot.timer = activeSlot.weapon.Cooldown;
        }
    }

    private void HandleWeaponSwitch()
    {
        for (int i = 0; i < equippedWeapons.Count && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectWeapon(i);
            }
        }
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= equippedWeapons.Count || index == activeWeaponIndex)
            return;

        activeWeaponIndex = index;
        Debug.Log($"Arma activa: Slot {activeWeaponIndex + 1}");
    }

    private Transform FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, targetRange, enemyLayer);
        Transform nearest = null;
        float minDistSqr = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            float distSqr = (hit.transform.position - transform.position).sqrMagnitude;
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                nearest = hit.transform;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetRange);
    }
}