using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    private bool mouseOver = false;
    private Transform player;

    protected I18n i18n = I18n.Get();

    protected GameObject tooltip;
    protected TextMesh tooltipText;

    public abstract string GetName();

    public virtual string GetTooltipFormat()
    {
        return FormatCodes.WHITE;
    }

    public virtual string TerminateTooltip()
    {
        return FormatCodes.COL_E;
    }

    public virtual void OnSpawn()
    {
        tooltip = new GameObject("tooltip");
        tooltip.transform.SetParent(transform, false);
        tooltip.transform.Translate(0F, 1F, 0F);
        tooltipText = tooltip.AddComponent<TextMesh>();
        tooltipText.anchor = TextAnchor.LowerCenter;
        tooltipText.characterSize = .25F;
        tooltipText.richText = true;
    }

    public virtual void Update()
    {
        if (!player)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
                this.player = player.transform;
        }

        List<string> tip = new List<string>();
        GetTooltip(tip);

        bool vis = IsTooltipVisible();
        if (vis && tooltip)
        {
            tooltipText.transform.LookAt(player);
            tooltipText.transform.Rotate(0F, 180F, 0F);

            tooltipText.text = (tip.Count > 0 ? TextFormatting.ParseToUnity(i18n.Translate(tip[0])) : "");
            for (int i = 1; i < tip.Count; i++)
                tooltipText.text += "\n" + TextFormatting.ParseToUnity(i18n.Translate(tip[i]));
        }
        else if (tooltip)
        {
            tooltipText.text = "";
        }
    }

    public virtual void GetTooltip(List<string> tooltip)
    {
        tooltip.Clear();

        tooltip.Add(GetTooltipFormat() + GetName() + TerminateTooltip());
    }

    private void OnMouseEnter()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }

    public virtual bool IsTooltipVisible()
    {
        return mouseOver;
    }
}
