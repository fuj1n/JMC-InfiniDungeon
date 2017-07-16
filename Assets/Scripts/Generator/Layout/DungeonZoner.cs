using System;

public class DungeonZoner : DungeonTile, IComparable<DungeonZoner>
{
    public ZonerType type = ZonerType.ROOM;

    public void Place(DungeonRoom src, DungeonRoom target, Random rand)
    {
        int sx, sy;
        int tx, ty;
        switch (type)
        {
            case ZonerType.ROOM:
                sx = src.width / 2;
                sy = src.height / 2;
                tx = target.width / 2;
                ty = target.height / 2;
                break;
            default:
                throw new ArgumentException("Invalid zoner type");
        }

        DungeonRoom.Placement<DungeonZoner> srcZoner = new DungeonRoom.Placement<DungeonZoner>(src, sx, sy, this);
        DungeonRoom.Placement<DungeonZoner> targetZoner = new DungeonRoom.Placement<DungeonZoner>(target, tx, ty, this);

        src.zoners.Add(srcZoner);
        target.zoners.Add(targetZoner);

        src.zonerLinks.Add(srcZoner, targetZoner);
        target.zonerLinks.Add(targetZoner, srcZoner);
    }

    public int CompareTo(DungeonZoner other)
    {
        return base.CompareTo(other);
    }

    [Serializable]
    public enum ZonerType
    {
        ROOM = 0
    }
}
