using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Image fader;

    public DungeonGenerator generatorTemplate;

    private Color fullOpaque;
    private Color fullTransparent;

    private GameObject generator;

    private void Start()
    {
        Color c = fader.color;
        fullOpaque = new Color(c.r, c.g, c.b, 1F);
        fullTransparent = new Color(c.r, c.g, c.b, 0F);

        generator = Instantiate(generatorTemplate.gameObject);
        fader.color = fullOpaque;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
