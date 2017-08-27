using System;
using UnityEngine;

public class DungeonZoner : DungeonTile, IComparable<DungeonZoner>
{
    [NonSerialized]
    public Vector3 zonerLink;

    [NonSerialized]
    public bool isZonerExit;

    private void OnMouseDown()
    {
        if (isZonerExit)
        {
            PlayerControllerBase.GetActiveInstance().OnKill();
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (Vector3.Distance(player.transform.position, transform.position) <= 5F)
            player.transform.position = zonerLink;

    }

    public int CompareTo(DungeonZoner other)
    {
        return base.CompareTo(other);
    }
}
