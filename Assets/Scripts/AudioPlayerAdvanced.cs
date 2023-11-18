using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform Transform;

    void Update()
    {
        if (Transform)
            this.transform.position = Transform.position;
    }
}
