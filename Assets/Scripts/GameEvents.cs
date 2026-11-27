using UnityEngine;

public record PlayerHealthChangedEvent(float CurrentHealth, float MaxHealth);
public record PlayerDiedEvent();
public record EnemyDiedEvent(int XpReward, Vector3 Position);
public record PlayerXPChangedEvent(float CurrentXP, float XpToNextLevel);
public record PlayerLevelUpEvent(int NewLevel);

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}