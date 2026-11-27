using UnityEngine;

public interface IWeapon
{
    float Cooldown { get; }
    void Fire(Transform target);
}