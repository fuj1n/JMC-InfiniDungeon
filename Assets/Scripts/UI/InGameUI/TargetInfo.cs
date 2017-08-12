using UnityEngine;
using UnityEngine.UI;

public class TargetInfo : MonoBehaviour
{
    private static I18n i18n = I18n.Get();

    private Text targetName;

    private RectTransform targetHP;
    private RectTransform targetHPBackground;
    private Text targetHPVal;

    private Image backgroundImage;
    private Color bgCol;

    private static readonly Color invisible = new Color(0F, 0F, 0F, 0F);

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        bgCol = backgroundImage.color;

        targetName = transform.Find("TargetName").GetComponent<Text>();
        targetHPBackground = transform.Find("BarHP").GetComponent<RectTransform>();
        targetHP = transform.Find("BarHP").Find("Status").GetComponent<RectTransform>();
        targetHPVal = transform.Find("BarHP").Find("Value").GetComponent<Text>();
    }

    private void Update()
    {
        backgroundImage.color = TargetTracker.target ? bgCol : invisible;

        if (!TargetTracker.target)
            return;

        targetName.text = i18n.Translate(TargetTracker.target.GetName());
        targetHP.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetHPBackground.rect.width * TargetTracker.target.life);
        targetHPVal.text = (int)(TargetTracker.target.life * TargetTracker.target.maxLife) + " / " + (int)TargetTracker.target.maxLife;
    }
}
