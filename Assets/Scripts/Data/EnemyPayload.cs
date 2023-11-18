using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Payload")]
public class EnemyPayload : ScriptableObject
{
    public int Value = 1;
    public GameObject Prefab;
}
