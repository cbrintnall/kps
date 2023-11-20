using System;
using UnityEngine;

public enum Alliance
{
    PLAYER = 1,
    ENEMY = 2
}

public class Faction
{
    public Alliance Alliances;

    public GameObject[] GetAllInGroup()
    {
        throw new NotImplementedException();
    }
}
