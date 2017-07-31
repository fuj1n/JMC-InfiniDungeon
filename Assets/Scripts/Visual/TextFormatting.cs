using UnityEngine;

public class TextFormatting
{
    public static readonly Color[] COLORS = new Color[]
    {
        Color.black, // Black
        new Color(0F, 0F, .5F), // Dark Blue
        new Color(0F, .5F, 0F), // Dark Green
        new Color(0F, .5F, .5F), // Dark Aqua
        new Color(.5F, 0F, 0F), // Dark Red
        new Color(.5F, 0F, .5F), // Dark Purple
        new Color(1F, .5F, 0F), // Gold
        new Color(.5F, .5F, .5F), // Grey
        new Color(.25F, .25F, .25F), // Dark Grey
        new Color(.25F, .25F, 1F), // Light Blue
        Color.green, // Lime Green
        Color.cyan, // Aqua
        Color.red, // Red
        Color.magenta, // Magenta
        Color.yellow, // Yellow
        Color.white // White
    };

    // Loosly converts the formatting system to Unity's native one, this allows bold, italic and colors, but they all must be terminated with the _E of same type
    public static string ParseToUnity(string text)
    {
        text = text.Replace(FormatCodes.BOLD, "<b>").Replace(FormatCodes.ITALIC, "<i>").Replace(FormatCodes.BOLD_E, "</b>").Replace(FormatCodes.ITALIC_E, "</i>");

        for (int i = 0; i < COLORS.Length; i++)
        {
            int rgb = Mathf.RoundToInt(COLORS[i].r * 255);
            rgb = (rgb << 8) + Mathf.RoundToInt(COLORS[i].g * 255);
            rgb = (rgb << 8) + Mathf.RoundToInt(COLORS[i].b * 255);

            text = text.Replace("\u00A7" + i.ToString("X").ToLower(), "<color=#" + rgb.ToString("X") + ">");
        }

        text = text.Replace(FormatCodes.COL_E, "</color>");

        return text;
    }
}

public struct FormatCodes
{
    // Basic styling attributes
    public const string RESET = "\u00A7r";
    public const string BOLD = "\u00A7b";
    public const string BOLD_E = "\u00A8b";
    public const string UNDERLINE = "\u00A7u";
    public const string STRIKETHROUGH = "\u00A7s";
    public const string ITALIC = "\u00A7i";
    public const string ITALIC_E = "\u00A8i";
    public const string OBFUSCATED = "\u00A7o";

    // Sizing
    public const string SIZEUP = "\u00A7+";
    public const string SIZEDOWN = "\u00A7-";
    public const string SIZE_E = "\u00A8s";

    // Colors
    public const string BLACK = "\u00A70";
    public const string DARK_BLUE = "\u00A71";
    public const string DARK_GREEN = "\u00A72";
    public const string DARK_AQUA = "\u00A73";
    public const string DARK_RED = "\u00A74";
    public const string DARK_PURPLE = "\u00A75";
    public const string GOLD = "\u00A76";
    public const string GREY = "\u00A77";
    public const string DARK_GREY = "\u00A78";
    public const string LIGHT_BLUE = "\u00A79";
    public const string GREEN = "\u00A7a";
    public const string CYAN = "\u00A7b";
    public const string RED = "\u00A7c";
    public const string MAGENTA = "\u00A7d";
    public const string YELLOW = "\u00A7e";
    public const string WHITE = "\u00A7f";
    public const string COL_E = "\u00A8c";
}
