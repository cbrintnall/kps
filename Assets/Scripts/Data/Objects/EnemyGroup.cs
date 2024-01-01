using UnityEngine;

[CreateAssetMenu(fileName = "Data/Enemy/EnemyGroup")]
public class EnemyGroup : ScriptableObject
{
    public EnemyDefinition Enemy;
    public int Count = 1;
}
