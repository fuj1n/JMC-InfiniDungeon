using UnityEngine;
using UnityEngine.UI;

public class TargetInfo : MonoBehaviour
{
    private static I18n i18n = I18n.Get();

    public float updateSpeed = 2F;

    private Text targetName;

    private RectTransform targetHP;
    private RectTransform targetHPBackground;
    private Text targetHPVal;

    private Image backgroundImage;
    private Color bgCol;

    private float hpValue;
    private EntityLiving targetOld;

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
        {
            targetOld = null;
            return;
        }

        if (targetOld != TargetTracker.target)
        {
            targetOld = TargetTracker.target;
            hpValue = TargetTracker.target.life;
        }

        hpValue = Mathf.Lerp(hpValue, TargetTracker.target.life, Time.deltaTime * updateSpeed);

        targetName.text = i18n.Translate(TargetTracker.target.GetName());
        targetHP.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetHPBackground.rect.width * hpValue);
        targetHPVal.text = (int)(TargetTracker.target.life * TargetTracker.target.maxLife) + " / " + (int)TargetTracker.target.maxLife;
    }
}
