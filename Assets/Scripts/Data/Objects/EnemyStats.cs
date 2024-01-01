using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyStats")]
[InlineEditor]
public class EnemyStats : ScriptableObject
{
    [Range(1, 100)]
    public int Health;

    [Range(1, 100)]
    public int Damage;

    [Range(0.0f, 100.0f)]
    public float MoveSpeed;
}
