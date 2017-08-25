using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    public static readonly Vector2 BOX_SIZE = new Vector2(100, 100);

    public static readonly Color UNAVAILABLE = new Color(1F, 0F, 0F);
    public static readonly Color UNUSABLE = new Color(.20F, .20F, .20F);
    public static readonly Color AVAILABLE = new Color(1F, 1F, 1F);

    public static readonly Color COOLDOWN_VALUE = new Color(1F, 1F, 0F);

    public float animationSpeed = 2F;

    private Image[] renderers;
    private RectTransform[] cooldownOverlays;
    private Text[] cooldowns;

    private void Start()
    {
        PlayerControllerBase controller = PlayerControllerBase.GetActiveInstance();

        SpellBase[] spells = controller.Spells;
        renderers = new Image[spells.Length];
        cooldownOverlays = new RectTransform[spells.Length];
        cooldowns = new Text[spells.Length];

        for (int i = 0; i < spells.Length; i++)
        {
            SpellBase spell = spells[i];

            GameObject go = new GameObject("spell");
            go.transform.SetParent(transform, false);

            renderers[i] = go.AddComponent<Image>();
            renderers[i].sprite = Resources.Load<Sprite>("Icons/" + spell.icon);

            RectTransform rect = go.GetComponent<RectTransform>();

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BOX_SIZE.x);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BOX_SIZE.y);

            GameObject spellOverlay = new GameObject("overlay");
            cooldownOverlays[i] = spellOverlay.AddComponent<RectTransform>();
            cooldownOverlays[i].SetParent(rect, false);

            cooldownOverlays[i].SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0F, 0F);

            spellOverlay.AddComponent<Image>().color = new Color(UNUSABLE.r, UNUSABLE.g, UNUSABLE.b, .5F);

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

            Color col = AVAILABLE;

            if (!spells[i].CompareLevelRequirement(controller))
                col = UNAVAILABLE;
            else if (!spells[i].VerifyCanCastSpell(controller, true))
                col = UNUSABLE;
            else
                col = AVAILABLE;

            renderers[i].color = Color.Lerp(renderers[i].color, col, Time.deltaTime * animationSpeed);

            if (controller.IsInCooldown(spells[i]))
                cooldownOverlays[i].SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0F, BOX_SIZE.y * controller.GetCooldown(spells[i]) / spells[i].cooldown);
        }
    }
}
