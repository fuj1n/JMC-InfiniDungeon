using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private static I18n i18n = I18n.Get();

    public float updateSpeed = 2F;

    private Text playerName;

    private RectTransform playerHP;
    private RectTransform playerHPBackground;
    private Text playerHPVal;

    private RectTransform playerXP;
    private RectTransform playerXPBackground;
    private Text playerXPVal;

    private Text playerLevel;
    private Text playerClass;

    private float hpValue;
    private float xpValue;

    private void Awake()
    {
        playerName = transform.Find("PlayerName").GetComponent<Text>();

        playerHPBackground = transform.Find("BarHP").GetComponent<RectTransform>();
        playerHP = transform.Find("BarHP").Find("Status").GetComponent<RectTransform>();
        playerHPVal = transform.Find("BarHP").Find("Value").GetComponent<Text>();

        playerXPBackground = transform.Find("BarXP").GetComponent<RectTransform>();
        playerXP = transform.Find("BarXP").Find("Status").GetComponent<RectTransform>();
        playerXPVal = transform.Find("BarXP").Find("Value").GetComponent<Text>();

        playerLevel = transform.Find("PlayerLevel").GetComponent<Text>();
        playerClass = transform.Find("PlayerClass").GetComponent<Text>();
    }

    private void Start()
    {
        PlayerControllerBase controller = PlayerControllerBase.GetActiveInstance();

        hpValue = controller.life;
        xpValue = (float)PlayerData.Instance.Experience / PlayerData.Instance.ExperienceToNextLevel;
    }

    private void Update()
    {
        PlayerControllerBase controller = PlayerControllerBase.GetActiveInstance();
        PlayerData data = PlayerData.Instance;

        playerName.text = data.name;

        hpValue = Mathf.Lerp(hpValue, controller.life, Time.deltaTime * updateSpeed);
        xpValue = Mathf.Lerp(xpValue, (float)data.Experience / data.ExperienceToNextLevel, Time.deltaTime * updateSpeed);

        playerHP.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerHPBackground.rect.width * hpValue);
        playerHPVal.text = (int)(controller.life * controller.maxLife) + " / " + (int)controller.maxLife;

        playerXP.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerXPBackground.rect.width * xpValue);
        playerXPVal.text = data.Experience + " / " + data.ExperienceToNextLevel;

        playerLevel.text = "Level " + data.level;
        playerClass.text = i18n.Translate("player.class." + data.playerClass);
    }
}
