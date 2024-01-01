using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Data/Enemy/Enemy")]
public class EnemyDefinition : ScriptableObject
{
    [HorizontalGroup("Enemy", 75)]
    [PreviewField(75)]
    [HideLabel]
    public GameObject Prefab;

    [HorizontalGroup("Enemy")]
    public EnemyStats Stats;
    public Color PrimaryColor;
    public Color SecondaryColor;

    [Range(1, 100)]
    public int Value = 1;

    public Enemy Create()
    {
        Enemy enemy = Instantiate(Prefab).GetComponent<Enemy>();

        enemy.Definition = this;

        return enemy;
    }
}
