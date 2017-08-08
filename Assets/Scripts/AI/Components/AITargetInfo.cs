using System;

public class AITargetInfo : AIComponent
{
    public const string ID = "info.target";

    [NonSerialized]
    public Targetable target;

    public override string GetId()
    {
        return ID;
    }
}
