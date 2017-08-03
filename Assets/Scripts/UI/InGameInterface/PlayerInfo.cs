using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private static I18n i18n = I18n.Get();

    private Text playerName;

    private RectTransform playerHP;
    private RectTransform playerHPBackground;
    private Text playerHPVal;

    //private RectTransform playerMP;
    //private RectTransform playerMPBackground;
    //private Text playerMPVal;

    private Text playerLevel;
    private Text playerClass;

    private void Awake()
    {
        playerName = transform.Find("PlayerName").GetComponent<Text>();

        playerHPBackground = transform.Find("BarHP").GetComponent<RectTransform>();
        playerHP = transform.Find("BarHP").Find("Status").GetComponent<RectTransform>();
        playerHPVal = transform.Find("BarHP").Find("Value").GetComponent<Text>();

        //playerMPBackground = transform.Find("BarMP").GetComponent<RectTransform>();
        //playerMP = transform.Find("BarMP").Find("Status").GetComponent<RectTransform>();
        //playerMPVal = transform.Find("BarMP").Find("Value").GetComponent<Text>();

        playerLevel = transform.Find("PlayerLevel").GetComponent<Text>();
        playerClass = transform.Find("PlayerClass").GetComponent<Text>();
    }

    private void Update()
    {
        PlayerControllerBase controller = PlayerControllerBase.GetActiveInstance();
        PlayerData data = PlayerData.Instance;

        playerName.text = data.name;
        playerHP.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerHPBackground.rect.width * (controller.life / data.GetMaxLife()));
        playerHPVal.text = (int)controller.life + " / " + (int)data.GetMaxLife();

        playerLevel.text = "Level " + data.level;
        playerClass.text = i18n.Translate("player.class." + data.playerClass);
    }
}
