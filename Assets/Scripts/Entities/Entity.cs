using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract string GetName();

    public string GetTooltipFormat()
    {
        return FormatCodes.WHITE;
    }

    public virtual void GetTooltip(List<string> tooltip)
    {
        tooltip.Clear();

        tooltip.Add(FormatCodes.RESET + GetTooltipFormat() + GetName());
    }
}
