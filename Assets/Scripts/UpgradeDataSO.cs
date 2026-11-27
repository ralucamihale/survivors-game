using UnityEngine;

public enum UpgradeType
{
    UnlockSpread,
    UnlockAoE,
    MoveSpeed,
    MaxHealth,
    SingleShotDamage,
    GlobalAttackSpeed,
    SpreadExtraProjectile,
    AoERadius
}

[CreateAssetMenu(fileName = "UpgradeDataSO", menuName = "Scriptable Objects/Upgrade Data")]
public class UpgradeDataSO : ScriptableObject
{
    public string title;
    [TextArea] public string description;
    public UpgradeType type;
    public float value = 1.2f;

}
