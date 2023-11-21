using System;
using Sirenix.Utilities;
using UnityEngine;

public enum Alliance
{
    PLAYER = 1,
    ENEMY = 2
}

public class Faction
{
    public static Faction PLAYER = new Faction(Alliance.PLAYER);
    public static Faction ENEMY = new Faction(Alliance.ENEMY);

    public Alliance Alliances;

    public Faction(params Alliance[] alliances)
    {
        alliances.ForEach(ally => Alliances &= ally);
    }

    public GameObject[] GetAllInGroup()
    {
        throw new NotImplementedException();
    }
}
