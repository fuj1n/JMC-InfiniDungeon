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
}

public struct FormatCodes
{
    // Basic styling attributes
    public const string RESET = "\u00A7r";
    public const string BOLD = "\u00A7b";
    public const string UNDERLINE = "\u00A7u";
    public const string STRIKETHROUGH = "\u00A7s";
    public const string ITALIC = "\u00A7i";
    public const string OBFUSCATED = "\u00A7o";

    // Sizing
    public const string SIZEUP = "\u00A7+";
    public const string SIZEDOWN = "\u00A7-";

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
}
