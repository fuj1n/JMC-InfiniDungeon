using UnityEngine;
using UnityEngine.UI;

public class CastBar : MonoBehaviour
{
    private RectTransform castBarBg;
    private RectTransform castBar;
    private Text castText;

    private Image background;
    private Color baseColor;


    private void Awake()
    {
        castBarBg = transform.GetComponent<RectTransform>();
        castBar = transform.Find("Progress").GetComponent<RectTransform>();
        castText = GetComponentInChildren<Text>();

        background = GetComponent<Image>();
        baseColor = background.color;
    }

    void Update()
    {
        PlayerControllerBase controller = PlayerControllerBase.GetActiveInstance();

        background.color = controller.Progress >= .0F ? baseColor : new Color(0F, 0F, 0F, 0F);

        if (controller.Progress >= .0F)
        {
            castText.text = Mathf.RoundToInt(controller.Progress * 100) + "%";
            castBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, castBarBg.rect.width * controller.Progress);
        }
    }
}
