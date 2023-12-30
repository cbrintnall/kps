using UnityEngine;

public class RoomMaster : MonoBehaviour
{
    public void StartRoom()
    {
        FindObjectOfType<RoundManager>().StartRound();
    }
}
