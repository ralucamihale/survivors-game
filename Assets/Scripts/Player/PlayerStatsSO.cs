using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlayerStats", menuName = "Scriptable Objects/Default Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    public float maxHealth = 100f;
    public float moveSpeed = 5f;
    public float baseXpToNextLevel = 10f;
}
