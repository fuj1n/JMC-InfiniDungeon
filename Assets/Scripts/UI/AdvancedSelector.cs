using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class AdvancedSelector : UIBehaviour, IPointerClickHandler, IEventSystemHandler
{
    public string groupId = "ungrouped";
    public string data = "nodata";
    public Image targetGraphic;
    public Color selectedColor;

    private Color unselectedColor;
    private bool isSelected = false;
    private static Dictionary<string, List<AdvancedSelector>> selectorGroups = new Dictionary<string, List<AdvancedSelector>>();
    private static Dictionary<string, List<SelectEvent>> selectorEvents = new Dictionary<string, List<SelectEvent>>();

    public delegate void SelectEvent(AdvancedSelector sel, EventAction act);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected)
            return;

        isSelected = true;

        if (targetGraphic)
            targetGraphic.color = selectedColor;

        foreach (AdvancedSelector sel in selectorGroups[groupId])
            if (sel != this)
                sel.Deselect();

        if (selectorEvents.ContainsKey(groupId))
            foreach (SelectEvent ev in selectorEvents[groupId])
                ev(this, EventAction.SELECT);
    }

    public void Deselect()
    {
        if (!isSelected)
            return;

        isSelected = false;

        if (targetGraphic)
            targetGraphic.color = unselectedColor;

        if (selectorEvents.ContainsKey(groupId))
            foreach (SelectEvent ev in selectorEvents[groupId])
                ev(this, EventAction.DESELECT);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    protected override void Awake()
    {
        base.Awake();

        if (!selectorGroups.ContainsKey(groupId))
            selectorGroups.Add(groupId, new List<AdvancedSelector>());

        selectorGroups[groupId].Add(this);

        if (targetGraphic)
            unselectedColor = targetGraphic.color;
    }

    public static void Subscribe(string group, SelectEvent sel)
    {
        if (!selectorEvents.ContainsKey(group))
            selectorEvents.Add(group, new List<SelectEvent>());

        selectorEvents[group].Add(sel);
    }

    public static void ResetState()
    {
        selectorEvents.Clear();
        selectorGroups.Clear();
    }

    public static AdvancedSelector FindSelected(string groupId)
    {
        if (!selectorGroups.ContainsKey(groupId))
            return null;
        return (from s in selectorGroups[groupId]
               where s.isSelected
               select s).SingleOrDefault(); 
    }

    public enum EventAction
    {
        SELECT, DESELECT
    }
}
