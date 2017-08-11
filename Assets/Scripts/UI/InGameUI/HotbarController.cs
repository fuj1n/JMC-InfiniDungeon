using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    public static readonly Color UNAVAILABLE = new Color(1F, 0F, 0F);
    public static readonly Color UNUSABLE = new Color(.20F, .20F, .20F);
    public static readonly Color AVAILABLE = new Color(1F, 1F, 1F);

    public static readonly Color COOLDOWN_VALUE = new Color(1F, 1F, 0F);

    private Image[] renderers;
    private Text[] cooldowns;

    private void Start()
    {
        PlayerControllerBase controller = PlayerControllerBase.GetActiveInstance();

        SpellBase[] spells = controller.Spells;
        renderers = new Image[spells.Length];
        cooldowns = new Text[spells.Length];

        for (int i = 0; i < spells.Length; i++)
        {
            SpellBase spell = spells[i];

            GameObject go = new GameObject("spell");
            go.transform.SetParent(transform, false);

            renderers[i] = go.AddComponent<Image>();
            renderers[i].sprite = Resources.Load<Sprite>("Icons/" + spell.icon);

            RectTransform rect = go.GetComponent<RectTransform>();

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100F);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100F);

            GameObject numberBackground = new GameObject("numberBackground");
            numberBackground.transform.SetParent(rect, false);

            Image numberBg = numberBackground.AddComponent<Image>();
            numberBg.color = new Color(0F, 0F, 0F, .75F);

            RectTransform bgRect = numberBackground.GetComponent<RectTransform>();

            bgRect.anchorMin = new Vector2(0F, 0F);
            bgRect.anchorMax = new Vector2(0F, 0F);
            bgRect.pivot = new Vector2(0F, 0F);
            bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30F);
            bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30F);

            GameObject hotbarNumber = new GameObject("hotbarNumber");
            Text numberText = hotbarNumber.AddComponent<Text>();
            numberText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            numberText.fontSize = 36;
            numberText.alignment = TextAnchor.MiddleCenter;
            numberText.transform.SetParent(numberBackground.transform, false);

            numberText.text = PlayerControllerBase.keyLabels[i];

            GameObject cooldownNumber = new GameObject("cooldownNumber");
            cooldowns[i] = cooldownNumber.AddComponent<Text>();
            cooldowns[i].font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            cooldowns[i].fontSize = 36;
            cooldowns[i].alignment = TextAnchor.MiddleCenter;
            cooldowns[i].transform.SetParent(rect, false);
            cooldowns[i].color = COOLDOWN_VALUE;
        }
    }

    private void Update()
    {
        PlayerControllerBase controller = PlayerControllerBase.GetActiveInstance();

        SpellBase[] spells = controller.Spells;

        for (int i = 0; i < spells.Length; i++)
        {
            cooldowns[i].text = controller.IsInCooldown(spells[i]) ? controller.GetCooldown(spells[i]).ToString("F1") : "";

            if (!spells[i].CompareLevelRequirement(controller))
                renderers[i].color = UNAVAILABLE;
            else if (controller.IsInCooldown(spells[i]) || !spells[i].VerifyCanCastSpell(controller, true))
                renderers[i].color = UNUSABLE;
            else
                renderers[i].color = AVAILABLE;
        }
    }
}
