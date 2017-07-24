using UnityEngine;

public class ConnectorRoom : DungeonRoom
{
    public ConnectorRoom(int width, int height) : base(width, height) { }

    public override GameObject Generate(float x, float y, float z)
    {
        //TODO custom floor generation
        return base.Generate(x, y, z);
    }
}
