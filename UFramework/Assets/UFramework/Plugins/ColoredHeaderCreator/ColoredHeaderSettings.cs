#if UNITY_EDITOR

using UnityEngine;

public enum FontStyleOptions
{
    Bold = 0,
    Normal,
    Italic,
    BoldAndItalic
}

public enum TextAlignmentOptions
{
    Center,
    Left,
    Right,
}

[System.Serializable]
public class HeaderSettings
{
    public string headerText = "New Header";
    public Color headerColor = Color.gray;

    [Space(15)] public TextAlignmentOptions textAlignmentOptions = TextAlignmentOptions.Center;
    public FontStyleOptions fontStyleOptions = FontStyleOptions.Bold;
    public float fontSize = 12.0f;
    public Color fontColor = Color.white;
    public bool dropShadow;
}

public class ColoredHeaderSettings : ScriptableObject
{
    public HeaderSettings headerSettings = new HeaderSettings();

    public void ResetSettings()
    {
        headerSettings.headerText = "New Header";
        headerSettings.headerColor = Color.gray;
        headerSettings.textAlignmentOptions = TextAlignmentOptions.Center;
        headerSettings.fontStyleOptions = FontStyleOptions.Bold;
        headerSettings.fontSize = 12.0f;
        headerSettings.fontColor = Color.white;
        headerSettings.dropShadow = false;
    }
}
#endif